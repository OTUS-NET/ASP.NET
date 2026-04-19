using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    private readonly PartnersController _partnersController;

    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();

        _partnersController = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(1), 100);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        //Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        //Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Which;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = notFoundResult.Value as ProblemDetails;

        problemDetails!.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, false);
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(1), 100);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        //Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        //Assert
        var unprocessableResult = result.Result.Should().BeOfType<UnprocessableEntityObjectResult>().Which;
        unprocessableResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = unprocessableResult.Value as ProblemDetails;

        problemDetails!.Title.Should().Be("Partner blocked");
        problemDetails.Detail.Should().Be($"Cannot create limit for a blocked partner.");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        //Arrange
        const int PROMO_CODE_LIMIT = 100;

        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true);
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(1), PROMO_CODE_LIMIT);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        //Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        //Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Which;
        createdResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
        var response = createdResult.Value as PartnerPromoCodeLimitResponse;

        response!.Limit.Should().Be(100);
        response.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        _partnerLimitsRepositoryMock.Verify(
            x => x.Add(It.Is<PartnerPromoCodeLimit>(l => l.Limit == PROMO_CODE_LIMIT && l.Partner.Id == partnerId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        //Arrange
        const int PROMO_CODE_LIMIT = 100;

        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);

        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(1), PROMO_CODE_LIMIT);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        var oldLimit = partner.PartnerLimits.First();

        //Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        //Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Which;
        createdResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
        var response = createdResult.Value as PartnerPromoCodeLimitResponse;

        oldLimit.CanceledAt.Should().NotBeNull();
        var newLimit = result.Value;

        _partnersRepositoryMock.Verify(
            x => x.Update(It.Is<Partner>(p => p.PartnerLimits.Any(l => l.Id == oldLimit.Id)), It.IsAny<CancellationToken>()),
            Times.Once);
        _partnerLimitsRepositoryMock.Verify(
            x => x.Add(It.Is<PartnerPromoCodeLimit>(l => l.Id == response.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, Guid.NewGuid(), isActive: true);
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(1), 100);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _partnersRepositoryMock.Setup(x => x.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException(typeof(Partner), partnerId));

        //Act
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        //Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private static Partner CreatePartner(
        Guid partnerId,
        bool isActive)
    {
        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.PartnerLimits, _ => [])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();
        return partner;
    }

    private static Partner CreatePartnerWithLimit(
        Guid partnerId,
        Guid limitId,
        bool isActive,
        DateTimeOffset? canceledAt = null)
    {
        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .Generate();

        var employee = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => Guid.NewGuid())
            .RuleFor(e => e.Role, role)
            .Generate();

        var limits = new List<PartnerPromoCodeLimit>();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, limits)
            .Generate();

        var limit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.Id, _ => limitId)
            .RuleFor(l => l.Partner, partner)
            .RuleFor(l => l.CanceledAt, _ => canceledAt)
            .RuleFor(l => l.CreatedAt, _ => DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(l => l.EndAt, _ => DateTimeOffset.UtcNow.AddDays(30))
            .RuleFor(l => l.Limit, 2)
            .Generate();

        limits.Add(limit);
        return partner;
    }
}
