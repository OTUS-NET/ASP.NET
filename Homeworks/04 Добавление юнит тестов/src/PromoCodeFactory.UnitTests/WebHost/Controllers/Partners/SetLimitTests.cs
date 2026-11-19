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
        var limit = 1;

        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);
        PartnerPromoCodeLimitCreateRequest fakeRequest = new(DateTime.UtcNow.AddYears(1), limit);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        ActionResult<PartnerPromoCodeLimitResponse> result = await _partnerController.CreateLimit(partnerId, fakeRequest, CancellationToken.None);


        CreatedAtActionResult responce = Assert.IsType<CreatedAtActionResult>(result.Result);
        PartnerPromoCodeLimitResponse correctReponce = Assert.IsType<PartnerPromoCodeLimitResponse>(responce.Value);
        Assert.Equal(201, responce.StatusCode);
        correctReponce.Limit.Should().Be(limit);
    }

    [Fact]
    public async Task CreateLimit_WhenValidRequestWithActiveLimits_CancelsOldLimitsAndAddsNew()
    {
        var partnerId = Guid.NewGuid();
        var oldLimitId = Guid.NewGuid();
        var newLimitValue = 10;
        PartnerPromoCodeLimit? addedLimit = null;
        DateTimeOffset endAt = DateTimeOffset.UtcNow.AddYears(1);

        Partner partner = CreatePartnerWithLimit(partnerId, oldLimitId, isActive: true);
        PartnerPromoCodeLimit oldLimit = partner.PartnerLimits.Single();
        PartnerPromoCodeLimitCreateRequest request = new(endAt, newLimitValue);

        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(partner, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _partnerLimitsRepositoryMock
            .Setup(r => r.Add(It.IsAny<PartnerPromoCodeLimit>(), It.IsAny<CancellationToken>()))
            .Callback<PartnerPromoCodeLimit, CancellationToken>((limit, _) => addedLimit = limit)
            .Returns(Task.CompletedTask);

        ActionResult<PartnerPromoCodeLimitResponse> result =
            await _partnerController.CreateLimit(partnerId, request, CancellationToken.None);

        CreatedAtActionResult response = Assert.IsType<CreatedAtActionResult>(result.Result);
        PartnerPromoCodeLimitResponse body = Assert.IsType<PartnerPromoCodeLimitResponse>(response.Value);
        Assert.Equal(201, response.StatusCode);

        oldLimit.CanceledAt.Should().NotBeNull();
        oldLimit.CanceledAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        _partnersRepositoryMock.Verify(
            r => r.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
        addedLimit.Should().NotBeNull();
        addedLimit!.Id.Should().NotBe(oldLimitId);
        addedLimit.Limit.Should().Be(newLimitValue);
        addedLimit.EndAt.Should().Be(endAt);
        addedLimit.CanceledAt.Should().BeNull();
        addedLimit.IssuedCount.Should().Be(0);
        addedLimit.Partner.Should().BeSameAs(partner);
        body.Id.Should().Be(addedLimit.Id);
        body.Limit.Should().Be(newLimitValue);
        _partnerLimitsRepositoryMock.Verify(
            r => r.Add(addedLimit, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    [Fact]
    public async Task CreateLimit_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var limit = 1;

        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);
        PartnerPromoCodeLimitCreateRequest fakeRequest = new(DateTime.UtcNow.AddYears(1), limit);
        _partnersRepositoryMock
            .Setup(r => r.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _partnersRepositoryMock
            .Setup(r => r.Update(partner, It.IsAny<CancellationToken>()))
            .Throws(new EntityNotFoundException(typeof(Partner), partner.Id));

        ActionResult<PartnerPromoCodeLimitResponse> result = await _partnerController.CreateLimit(partnerId, fakeRequest, CancellationToken.None);

        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
        Assert.Equal(404, notFoundResult.StatusCode);
        problemDetails.Title.Should().Be($"Partner not found");
        problemDetails.Detail.Should().Be($"{typeof(Partner).Name} with Id '{partner.Id}' was not found.");

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
