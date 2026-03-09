using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;
using PromoCodeFactory.WebHost.Controllers;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class CancelLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    private readonly PartnersController _sut;

    public CancelLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _sut = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }

    [Fact]
    public async Task CancelLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.CancelLimit(partnerId, limitId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Partner not found");
    }

    [Fact]
    public async Task CancelLimit_WhenPartnerBlocked_ReturnsBadRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, limitId, isActive: false);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.CancelLimit(partnerId, limitId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)badRequestResult.Value!;
        problemDetails.Title.Should().Be("Partner blocked");
    }

    [Fact]
    public async Task CancelLimit_WhenLimitNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var otherLimitId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, otherLimitId, isActive: true);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.CancelLimit(partnerId, limitId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().Be("Limit not found");
    }

    [Fact]
    public async Task CancelLimit_WhenLimitAlreadyCanceled_ReturnsBadRequest()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true, canceledAt: DateTimeOffset.UtcNow);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        // Act
        var result = await _sut.CancelLimit(partnerId, limitId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)badRequestResult.Value!;
        problemDetails.Title.Should().Be("Limit already canceled");
    }

    [Fact]
    public async Task CancelLimit_WhenValidRequest_ReturnsNoContentAndUpdatesPartner()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CancelLimit(partnerId, limitId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        partner.PartnerLimits.First().CanceledAt.Should().NotBeNull();
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Partner>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Partner>(partnerId));

        // Act
        var result = await _sut.CancelLimit(partnerId, limitId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
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
            .Generate();

        limits.Add(limit);
        return partner;
    }
}
