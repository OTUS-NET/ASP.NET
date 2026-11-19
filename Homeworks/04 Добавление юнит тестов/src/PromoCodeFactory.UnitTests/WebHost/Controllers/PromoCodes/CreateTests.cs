using AwesomeAssertions;
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
    private readonly PromoCodesController _promoCodeController;
    private readonly Mock<IRepository<PromoCode>> _mockPromoCodesRepository;
    private readonly Mock<IRepository<Customer>> _mockCustomersRepository;
    private readonly Mock<IRepository<CustomerPromoCode>> _mockCustomerPromoCodesRepository;
    private readonly Mock<IRepository<Partner>> _mockPartnersRepository;
    private readonly Mock<IRepository<Preference>> _mockPreferencesRepository;

    public CreateTests()
    {
        _mockPromoCodesRepository = new Mock<IRepository<PromoCode>>();
        _mockCustomersRepository = new Mock<IRepository<Customer>>();
        _mockCustomerPromoCodesRepository = new Mock<IRepository<CustomerPromoCode>>();
        _mockPartnersRepository = new Mock<IRepository<Partner>>();
        _mockPreferencesRepository = new Mock<IRepository<Preference>>();
        _promoCodeController = new PromoCodesController(_mockPromoCodesRepository.Object,
            _mockCustomersRepository.Object,
            _mockCustomerPromoCodesRepository.Object,
            _mockPartnersRepository.Object,
            _mockPreferencesRepository.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();
        var request = new PromoCodeCreateRequest(
            string.Empty,
            string.Empty,
            partnerId,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            new Guid());
        _mockPartnersRepository.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        ActionResult<PromoCodeShortResponse> responce = await _promoCodeController.Create(request, CancellationToken.None);

        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(responce.Result);
        ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
        Assert.Equal(404, notFoundResult.StatusCode);
        problemDetails.Title.Should().Be($"Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: false);
        var request = new PromoCodeCreateRequest(
            string.Empty,
            string.Empty,
            partnerId,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            preferenceId);

        _mockPartnersRepository.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _mockPreferencesRepository.Setup(x => x.GetById(request.PreferenceId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        ActionResult<PromoCodeShortResponse> responce = await _promoCodeController.Create(request, CancellationToken.None);
        NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(responce.Result);
        ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
        Assert.Equal(404, notFoundResult.StatusCode);
        problemDetails.Title.Should().Be($"Preference not found");
        problemDetails.Detail.Should().Be($"Preference with Id {request.PreferenceId} not found.");

    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: false, DateTime.Now);
        Preference preference = CreatePreference(preferenceId);
        var request = new PromoCodeCreateRequest(
            string.Empty,
            string.Empty,
            partnerId,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            preferenceId);

        _mockPartnersRepository.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _mockPreferencesRepository.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        ActionResult<PromoCodeShortResponse> responce = await _promoCodeController.Create(request, CancellationToken.None);

        //responce.Should().BeOfType<UnprocessableEntityObjectResult>();
        ObjectResult badRequestResult = Assert.IsType<ObjectResult>(responce.Result);
        badRequestResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)badRequestResult.Value!;
        problemDetails.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");

    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        Assert.Fail();
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        Assert.Fail();
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

    private Preference CreatePreference(Guid preferenceId)
    {
        Preference preference = new AutoFaker<Preference>()
            .RuleFor(x => x.Id, _ => preferenceId)
            .RuleFor(x => x.Name, "TestName")
            .RuleFor(x => x.Customers, new List<Customer>())
            .Generate();

        return preference;
    }
}
