using AwesomeAssertions;
using Bogus;
using Bogus.DataSets;
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
    private readonly PartnersController _sut;

    public SetLimitTests()
    {
        _partnersRepositoryMock = new();
        _partnerLimitsRepositoryMock = new();
        _sut = new(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateLimitRequest();
        _partnersRepositoryMock
            .Setup(p => p.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result?.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result!.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateLimitRequest();
        var partner = CreatePartner(partnerId, isActive: false);
        _partnersRepositoryMock
            .Setup(p => p.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        result?.Result.Should().BeOfType<UnprocessableEntityObjectResult>();
        var unprocessableEntityResult = (UnprocessableEntityObjectResult)result!.Result!;
        unprocessableEntityResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)unprocessableEntityResult.Value!;
        problemDetails?.Title.Should().Be("Partner blocked");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateLimitRequest();
        var partner = CreatePartner(partnerId, isActive: true);
        PartnerPromoCodeLimit? capturedCreatedLimit = null;
        _partnersRepositoryMock
            .Setup(p => p.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _partnersRepositoryMock
            .Setup(p => p.Update(partner, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _partnerLimitsRepositoryMock
            .Setup(l => l.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Callback<PartnerPromoCodeLimit, CancellationToken>((l, _) => capturedCreatedLimit = l)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        _partnerLimitsRepositoryMock.Verify(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()), Times.Once);
        capturedCreatedLimit.Should().NotBeNull();
        result?.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result!.Result!;
        createdAtActionResult.ActionName.Should().Be(nameof(_sut.GetLimit));
        var isRouteValuesValid = createdAtActionResult.RouteValues?.ToArray() switch
        {
            [{ Value: Guid pid }, { Value: Guid lid }] when pid == partnerId && lid == capturedCreatedLimit.Id => true,
            _ => false
        };

        isRouteValuesValid.Should().BeTrue();
        createdAtActionResult.Value.Should().BeOfType<PartnerPromoCodeLimitResponse>();
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateLimitRequest();
        var partner = CreatePartner(partnerId, isActive: true);
        PartnerPromoCodeLimit? capturedLimit = null;
        _partnersRepositoryMock.Setup(p => p.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _partnersRepositoryMock.Setup(p => p.Update(partner, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _partnerLimitsRepositoryMock.Setup(l => l.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Callback<PartnerPromoCodeLimit, CancellationToken>((l, _) => capturedLimit = l)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        partner.PartnerLimits.ElementAt(0).CanceledAt.Should().NotBeNull();
        capturedLimit.Should().NotBeNull();
        capturedLimit!.CanceledAt.Should().BeNull();
        capturedLimit!.Partner.Should().Be(partner);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var request = CreateLimitRequest();
        var partner = CreatePartner(partnerId, isActive: true);
        _partnersRepositoryMock
            .Setup(p => p.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _partnersRepositoryMock
            .Setup(p => p.Update(partner, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));
        _partnerLimitsRepositoryMock
            .Setup(l => l.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateLimit(partnerId, request, CancellationToken.None);

        // Assert
        _partnersRepositoryMock.Verify(r => r.Update(partner, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        result?.Result.Should().BeOfType<NotFoundResult>();
    }

    private PartnerPromoCodeLimitCreateRequest CreateLimitRequest()
    {
        var faker = new Faker();
        return new(EndAt: faker.Date.FutureOffset(), Limit: faker.Random.Int(1, int.MaxValue));
    }

    private Partner CreatePartner(Guid id, bool isActive = true)
    {
        var faker = new Faker();
        var gender = (Name.Gender)faker.Random.Int(0, 1);

        var role = new AutoFaker<Role>()
            .RuleFor(r => r.Name, _ => "PartnerManager")
            .RuleFor(r => r.Description, _ => "Партнерский менеджер")
            .Generate();

        var manager = new AutoFaker<Employee>()
            .RuleFor(e => e.FirstName, f => f.Name.FirstName(gender))
            .RuleFor(e => e.LastName, f => f.Name.LastName(gender))
            .RuleFor(e => e.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName, "example.com", $"{f.UniqueIndex}"))
            .RuleFor(e => e.Role, _ => role)
            .Generate();

        var limit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(l => l.CreatedAt, f => f.Date.PastOffset())
            .RuleFor(l => l.EndAt, f => f.Date.FutureOffset())
            .RuleFor(l => l.CanceledAt, _ => null)
            .RuleFor(l => l.Limit, f => f.Random.Int(min: 2))
            .RuleFor(l => l.IssuedCount, (f, l) => f.Random.Int(min: 0, max: l.Limit - 1))
            .Generate();

        return new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => id)
            .RuleFor(p => p.Name, f => f.Company.CompanyName())
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, _ => manager)
            .RuleFor(p => p.PartnerLimits, _ => [limit])
            .Generate();
    }
}
