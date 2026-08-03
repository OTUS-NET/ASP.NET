using System.Linq.Expressions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodesRepositoryMock;
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly PromoCodesController _promoCodesController;

    public CreateTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _promoCodesController = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            _customerPromoCodesRepositoryMock.Object,
            _partnersRepositoryMock.Object,
            _preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var request = new PromoCodeCreateRequest(
            Code: "PROMO2026",
            ServiceInfo: "Test service",
            PartnerId: partnerId,
            BeginDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            PreferenceId: Guid.NewGuid());

        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>())).ReturnsAsync((Partner?)null);

        /*Act*/
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        /*Assert*/
        var notFound = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFound.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.PartnerLimits, _ => [])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();

        var request = new PromoCodeCreateRequest(
            Code: "PROMO2026",
            ServiceInfo: "Test service",
            PartnerId: partnerId,
            BeginDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            PreferenceId: preferenceId);

        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>())).ReturnsAsync(partner);

        _preferencesRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>())).ReturnsAsync((Preference?)null);

        /*Act*/
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        /*Assert*/
        var notFound = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problemDetails = notFound.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Preference not found");
        problemDetails.Detail.Should().Be($"Preference with Id {preferenceId} not found.");
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();

        var canceledLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.CanceledAt, _ => DateTimeOffset.UtcNow.AddDays(-1))
            .RuleFor(x => x.EndAt, _ => DateTimeOffset.UtcNow.AddDays(10))
            .Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.PartnerLimits, _ => [canceledLimit])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();

        var preference = new AutoFaker<Preference>().Generate();

        var request = new PromoCodeCreateRequest(
            Code: "PROMO2026",
            ServiceInfo: "Test service",
            PartnerId: partnerId,
            BeginDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            PreferenceId: preferenceId);

        _partnersRepositoryMock
            .Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);

        _preferencesRepositoryMock
            .Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(x => x.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Customer>());

        /*Act*/
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        /*Assert*/
        var unprocessable = result.Result.Should().BeOfType<ObjectResult>().Subject;
        unprocessable.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var problemDetails = unprocessable.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var issuedCount = 5;
        var limit = 5;

        var activeLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.CanceledAt, _ => null)
            .RuleFor(x => x.EndAt, _ => DateTimeOffset.UtcNow.AddDays(10))
            .RuleFor(x => x.IssuedCount, _ => issuedCount)
            .RuleFor(x => x.Limit, _ => limit)
            .Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.PartnerLimits, _ => [activeLimit])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();

        var preference = new AutoFaker<Preference>().Generate();

        var request = new PromoCodeCreateRequest(
            Code: "PROMO2026",
            ServiceInfo: "Test service",
            PartnerId: partnerId,
            BeginDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            PreferenceId: preferenceId);

        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>())).ReturnsAsync(partner);

        _preferencesRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>())).ReturnsAsync(preference);

        _customersRepositoryMock.Setup(x => x.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Customer>());

        /*Act*/
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        /*Assert*/
        var unprocessable = result.Result.Should().BeOfType<ObjectResult>().Subject;
        unprocessable.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
        var problemDetails = unprocessable.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Limit exceeded");
        problemDetails.Detail.Should().Be($"Cannot create promo code. Limit would be exceeded (current: {issuedCount}/{limit}).");
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        /*Arrange*/
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var initialIssuedCount = 3;

        var activeLimit = new AutoFaker<PartnerPromoCodeLimit>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.CanceledAt, _ => null)
            .RuleFor(x => x.EndAt, _ => DateTimeOffset.UtcNow.AddDays(10))
            .RuleFor(x => x.IssuedCount, _ => initialIssuedCount)
            .RuleFor(x => x.Limit, _ => 10)
            .Generate();

        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.PartnerLimits, _ => [activeLimit])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();

        var preference = new AutoFaker<Preference>()
            .RuleFor(x => x.Id, _ => preferenceId)
            .Generate();

        var request = new PromoCodeCreateRequest(
            Code: "PROMO2026",
            ServiceInfo: "Test service",
            PartnerId: partnerId,
            BeginDate: DateTimeOffset.UtcNow,
            EndDate: DateTimeOffset.UtcNow.AddDays(30),
            PreferenceId: preferenceId);

        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>())).ReturnsAsync(partner);
        _preferencesRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>())).ReturnsAsync(preference);
        _customersRepositoryMock.Setup(x => x.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(Array.Empty<Customer>());

        PromoCode? addedPromoCode = null;
        _promoCodesRepositoryMock
            .Setup(x => x.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Callback<PromoCode, CancellationToken>((pc, _) => addedPromoCode = pc)
            .Returns(Task.CompletedTask);

        _partnersRepositoryMock.Setup(x => x.Update(partner, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        /*Act*/
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        /*Assert*/
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(PromoCodesController.GetById));

        addedPromoCode.Should().NotBeNull();
        addedPromoCode!.Id.Should().NotBeEmpty();
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(addedPromoCode.Id);
        addedPromoCode.Code.Should().Be(request.Code);
        addedPromoCode.ServiceInfo.Should().Be(request.ServiceInfo);
        addedPromoCode.Partner.Should().Be(partner);
        addedPromoCode.Preference.Should().Be(preference);

        var response = createdResult.Value.Should().BeOfType<PromoCodeShortResponse>().Subject;
        response.Id.Should().Be(addedPromoCode.Id);
        response.Code.Should().Be(request.Code);
        response.PartnerId.Should().Be(partnerId);
        response.PreferenceId.Should().Be(preferenceId);

        activeLimit.IssuedCount.Should().Be(initialIssuedCount + 1);

        _promoCodesRepositoryMock.Verify(x => x.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()), Times.Once);
        _partnersRepositoryMock.Verify(x => x.Update(partner, It.IsAny<CancellationToken>()), Times.Once);
    }
}
