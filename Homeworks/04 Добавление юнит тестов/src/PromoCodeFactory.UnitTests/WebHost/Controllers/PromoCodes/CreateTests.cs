using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly PromoCodesController _promoCodeController;
    private readonly Mock<IRepository<PromoCode>> _mockPromoCodesRepository;
    private readonly Mock<IRepository<Customer>> _mockCustomersRepository;
    private readonly Mock<IRepository<CustomerPromoCode>> _mockCustomerPromoCodesRepository;
    private readonly Mock<IRepository<Partner>> _mockPartnersRepository;
    private readonly Mock<IRepository<Preference>> _mockPreferencesRepositor;

    public CreateTests()
    {
        _mockPromoCodesRepository = new Mock<IRepository<PromoCode>>();
        _mockCustomersRepository = new Mock<IRepository<Customer>>();
        _mockCustomerPromoCodesRepository = new Mock<IRepository<CustomerPromoCode>>();
        _mockPartnersRepository = new Mock<IRepository<Partner>>();
        _mockPreferencesRepositor = new Mock<IRepository<Preference>>();
        _promoCodeController = new PromoCodesController(_mockPromoCodesRepository.Object,
            _mockCustomersRepository.Object,
            _mockCustomerPromoCodesRepository.Object,
            _mockPartnersRepository.Object,
            _mockPreferencesRepositor.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
    }
}
