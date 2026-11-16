using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.Partners;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetLimitTests
{
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PartnerPromoCodeLimit>> _partnerLimitsRepositoryMock;
    private readonly PartnersController _partnerController;

    public SetLimitTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _partnerLimitsRepositoryMock = new Mock<IRepository<PartnerPromoCodeLimit>>();
        _partnerController = new PartnersController(_partnersRepositoryMock.Object, _partnerLimitsRepositoryMock.Object);
    }




    [Fact]
    public async Task CreateLimit_WhenPartnerNotFound_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();
        PartnerPromoCodeLimitCreateRequest fakeRequest = new(DateTime.UtcNow.AddYears(1), 1);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        ActionResult<PartnerPromoCodeLimitResponse> result = await _partnerController.CreateLimit(partnerId, fakeRequest, CancellationToken.None);

        // Assert

        // 1. Проверяем, что возвращен тип NotFoundObjectResult (404 с телом ответа)
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
        Assert.Equal(404, notFoundResult.StatusCode);
        problemDetails.Title.Should().Be($"Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");

    }

    [Fact]
    public async Task CreateLimit_WhenPartnerBlocked_ReturnsUnprocessableEntity()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();

        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: false);
        PartnerPromoCodeLimitCreateRequest fakeRequest = new(DateTime.UtcNow.AddYears(1), 1);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        ActionResult<PartnerPromoCodeLimitResponse> result = await _partnerController.CreateLimit(partnerId, fakeRequest, CancellationToken.None);

        UnprocessableEntityObjectResult responce = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
        ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(responce.Value);
        Assert.Equal(422, responce.StatusCode);
        problemDetails.Title.Should().Be("Partner blocked");
        problemDetails.Detail.Should().Be("Cannot create limit for a blocked partner.");
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequest_ReturnsCreatedAndAddsLimit()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        int limit = 1;

        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);
        PartnerPromoCodeLimitCreateRequest fakeRequest = new(DateTime.UtcNow.AddYears(1), limit);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        ActionResult<PartnerPromoCodeLimitResponse> result = await _partnerController.CreateLimit(partnerId, fakeRequest, CancellationToken.None);


        var responce = Assert.IsType<CreatedAtActionResult>(result.Result);
        PartnerPromoCodeLimitResponse correctReponce = Assert.IsType<PartnerPromoCodeLimitResponse>(responce.Value);
        Assert.Equal(201, responce.StatusCode);
        correctReponce.Limit.Should().Be(limit);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        int limit = 1;

        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);
        PartnerPromoCodeLimitCreateRequest fakeRequest = new(DateTime.UtcNow.AddYears(1), limit);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        ActionResult<PartnerPromoCodeLimitResponse> result = await _partnerController.CreateLimit(partnerId, fakeRequest, CancellationToken.None);
    }

    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
    }

    private static Partner CreatePartnerWithLimit(
    Guid partnerId,
    Guid limitId,
    bool isActive,
    DateTimeOffset? canceledAt = null)
    {
        Role role = new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => Guid.NewGuid())
            .Generate();

        Employee employee = new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => Guid.NewGuid())
            .RuleFor(e => e.Role, role)
            .Generate();

        var limits = new List<PartnerPromoCodeLimit>();
        Partner partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.Manager, employee)
            .RuleFor(p => p.PartnerLimits, limits)
            .Generate();

        PartnerPromoCodeLimit limit = new AutoFaker<PartnerPromoCodeLimit>()
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
