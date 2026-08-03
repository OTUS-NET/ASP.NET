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
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(2), 100);
        _partnersRepositoryMock
            .Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        /*Act*/
        var res = await _partnersController.CreateLimit(partnerId: partnerId, request, CancellationToken.None);

        /*Assert*/
        var foundResult = res.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetail = foundResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetail.Title.Should().Be("Partner not found");
        problemDetail.Detail.Should().Be($"Partner with Id {partnerId} not found.");
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, false);
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(2), 100);
        _partnersRepositoryMock
            .Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        /*Act*/
        var res = await _partnersController.CreateLimit(partnerId: partnerId, request, CancellationToken.None);

        /*Assert*/
        var unprocessableEntity = res.Result.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
        var problemDetail = unprocessableEntity.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetail.Title.Should().Be("Partner blocked");
        problemDetail.Detail.Should().Be("Cannot create limit for a blocked partner.");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true);
        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(2), 100);
        _partnersRepositoryMock
            .Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        PartnerPromoCodeLimit? partnerLimit = null;

        _partnerLimitsRepositoryMock
            .Setup(x=>x.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Callback((PartnerPromoCodeLimit? limit, CancellationToken token) => partnerLimit = limit)
            .Returns(Task.CompletedTask);

        /*Act*/
        var res = await _partnersController.CreateLimit(partnerId: partnerId, request, CancellationToken.None);

        /*Assert*/
        var createdResult = res.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(PartnersController.GetLimit));
        createdResult.RouteValues.Should().ContainKey("partnerId").WhoseValue.Should().Be(partnerId);

        var response = createdResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>().Subject;
        response.Limit.Should().Be(request.Limit);
        response.EndAt.Should().Be(request.EndAt);

        partnerLimit.Should().NotBeNull();
        partnerLimit!.Id.Should().NotBeEmpty();
        createdResult.RouteValues.Should().ContainKey("limitId").WhoseValue.Should().Be(partnerLimit!.Id);
        partnerLimit.Partner.Should().Be(partner);
        partnerLimit!.Limit.Should().Be(request.Limit);
        partnerLimit.EndAt.Should().Be(request.EndAt);
        partnerLimit.IssuedCount.Should().Be(0);
        partnerLimit.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        _partnerLimitsRepositoryMock.Verify(x => x.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        /*Arrange*/
    var partnerId = Guid.NewGuid();
    var partner = CreatePartner(partnerId, true);

    var oldLimit = new AutoFaker<PartnerPromoCodeLimit>()
        .RuleFor(x => x.Id, _ => Guid.NewGuid())
        .RuleFor(x => x.Partner, _ => partner)
        .RuleFor(x => x.CanceledAt, _ => null)
        .Generate();

    partner.PartnerLimits.Add(oldLimit);

    var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(2), 100);

    _partnersRepositoryMock.Setup(
            x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
        .ReturnsAsync(partner);

    _partnersRepositoryMock.Setup(
            x => x.Update(partner, It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    PartnerPromoCodeLimit? addedLimit = null;

    _partnerLimitsRepositoryMock
        .Setup(x => x.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
        .Callback<PartnerPromoCodeLimit, CancellationToken>((limit, _) =>
        {
            addedLimit = limit;
        });
    /*Act*/
    var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

    /*Assert*/
    result.Result.Should().BeOfType<CreatedAtActionResult>();

    oldLimit.CanceledAt.Should().NotBeNull();
    oldLimit.CanceledAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));

    _partnersRepositoryMock.Verify(x => x.Update(partner, It.IsAny<CancellationToken>()), Times.Once);

    addedLimit.Should().NotBeNull();
    addedLimit!.Id.Should().NotBeEmpty();
    addedLimit.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    addedLimit!.Partner.Should().Be(partner);
    addedLimit.Limit.Should().Be(request.Limit);
    addedLimit.EndAt.Should().Be(request.EndAt);
    addedLimit.IssuedCount.Should().Be(0);
    _partnerLimitsRepositoryMock.Verify(x => x.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true);

        var oldLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Partner, _ => partner)
            .RuleFor(x => x.CanceledAt, _ => null)
            .Generate();

        partner.PartnerLimits.Add(oldLimit);

        var request = new PartnerPromoCodeLimitCreateRequest(DateTimeOffset.UtcNow.AddDays(2), 100);

        _partnersRepositoryMock.Setup(
                x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock.Setup(
                x => x.Update(partner, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));

        /*Act*/
        var result = await _partnersController.CreateLimit(partnerId, request, CancellationToken.None);

        /*Assert*/
        result.Result.Should().BeOfType<NotFoundResult>();

        _partnerLimitsRepositoryMock.Verify(x => x.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    private static Partner CreatePartner(Guid partnerId, bool isActive)
    {
        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.PartnerLimits, _ => [])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();
        return partner;
    }
}
