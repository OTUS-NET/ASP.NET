using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;
using System.Linq.Expressions;

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

        ObjectResult badRequestResult = Assert.IsType<ObjectResult>(responce.Result);
        badRequestResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)badRequestResult.Value!;
        problemDetails.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");

    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        Partner partner = CreatePartnerWithLimitItem(partnerId, limitId, isActive: false);
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

        _mockCustomersRepository
            .Setup(x => x.GetWhere(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        ActionResult<PromoCodeShortResponse> responce = await _promoCodeController.Create(request, CancellationToken.None);

        ObjectResult result = Assert.IsType<ObjectResult>(responce.Result);
        ProblemDetails problemDetails = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(422, result.StatusCode);
        problemDetails.Title.Should().Be("Limit exceeded");
        problemDetails.Detail.Should().Be(
            $"Cannot create promo code. Limit would be exceeded (current: {partner.PartnerLimits.First().IssuedCount}/{partner.PartnerLimits.First().Limit}).");
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        var partnerId = Guid.NewGuid();
        var limitId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        const int issuedCount = 2;
        const int limit = 5;
        const string code = "SPRING";
        const string serviceInfo = "Test service";
        var beginDate = DateTimeOffset.UtcNow;
        var endDate = beginDate.AddDays(1);

        Partner partner = CreatePartnerWithLimit(partnerId, limitId, isActive: true);
        PartnerPromoCodeLimit activeLimit = partner.PartnerLimits.Single();
        activeLimit.IssuedCount = issuedCount;
        activeLimit.Limit = limit;

        Preference preference = CreatePreference(preferenceId);
        Customer customer = new AutoFaker<Customer>()
            .RuleFor(c => c.Preferences, _ => new List<Preference> { preference })
            .RuleFor(c => c.CustomerPromoCodes, _ => [])
            .Generate();

        var request = new PromoCodeCreateRequest(
            code,
            serviceInfo,
            partnerId,
            beginDate,
            endDate,
            preferenceId);

        PromoCode? addedPromoCode = null;

        _mockPartnersRepository.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _mockPreferencesRepository.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _mockCustomersRepository
            .Setup(x => x.GetWhere(
                It.IsAny<Expression<Func<Customer, bool>>>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer> { customer });
        _mockPromoCodesRepository
            .Setup(x => x.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Callback<PromoCode, CancellationToken>((promoCode, _) => addedPromoCode = promoCode)
            .Returns(Task.CompletedTask);
        _mockPartnersRepository
            .Setup(x => x.Update(partner, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        ActionResult<PromoCodeShortResponse> response =
            await _promoCodeController.Create(request, CancellationToken.None);

        CreatedAtActionResult created = Assert.IsType<CreatedAtActionResult>(response.Result);
        PromoCodeShortResponse body = Assert.IsType<PromoCodeShortResponse>(created.Value);
        Assert.Equal(201, created.StatusCode);
        created.ActionName.Should().Be(nameof(PromoCodesController.GetById));

        addedPromoCode.Should().NotBeNull();
        addedPromoCode!.Code.Should().Be(code);
        addedPromoCode.ServiceInfo.Should().Be(serviceInfo);
        addedPromoCode.Partner.Should().BeSameAs(partner);
        addedPromoCode.Preference.Should().BeSameAs(preference);
        addedPromoCode.BeginDate.Should().Be(beginDate);
        addedPromoCode.EndDate.Should().Be(endDate);
        addedPromoCode.CustomerPromoCodes.Should().ContainSingle()
            .Which.CustomerId.Should().Be(customer.Id);

        activeLimit.IssuedCount.Should().Be(issuedCount + 1);

        body.Id.Should().Be(addedPromoCode.Id);
        body.Code.Should().Be(code);
        body.ServiceInfo.Should().Be(serviceInfo);
        body.PartnerId.Should().Be(partnerId);
        body.PreferenceId.Should().Be(preferenceId);
        created.RouteValues!["id"].Should().Be(addedPromoCode.Id);

        _mockPromoCodesRepository.Verify(
            x => x.Add(addedPromoCode, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockPartnersRepository.Verify(
            x => x.Update(partner, It.IsAny<CancellationToken>()),
            Times.Once);
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

    private static Partner CreatePartnerWithLimitItem(
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
            .RuleFor(x => x.IssuedCount, 1000)
            .RuleFor(x => x.Limit, 1)
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
