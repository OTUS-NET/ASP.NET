using AwesomeAssertions;
using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;
using System.Linq.Expressions;
using System.Net;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodeRepositoryMock = new();
    private readonly Mock<IRepository<Customer>> _customerRepositoryMock = new();
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodeRepositoryMock = new();
    private readonly Mock<IRepository<Partner>> _partnerRepositoryMock = new();
    private readonly Mock<IRepository<Preference>> _preferenceRepositoryMock = new();
    private readonly PromoCodesController _sut;

    public CreateTests()
    {
        _sut = new(
            _promoCodeRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _customerPromoCodeRepositoryMock.Object,
            _partnerRepositoryMock.Object,
            _preferenceRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var request = CreateRequest(partnerId, preferenceId);
        _partnerRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result?.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result!.Result!;
        var problemDetails = notFoundResult.Value as ProblemDetails;
        problemDetails?.Title.Should().NotBeNullOrEmpty();
        problemDetails!.Detail.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId);
        var request = CreateRequest(partnerId, preferenceId);
        _preferenceRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result?.Result?.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result!.Result!;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)notFoundResult.Value!;
        problemDetails.Title.Should().NotBeNullOrEmpty();
        problemDetails.Detail.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, hasNotCanceledLimit: false);
        var preference = CreatePreference(preferenceId);
        var request = CreateRequest(partnerId, preferenceId);
        _partnerRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _customerRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCustomerSet(preference).ToArray());

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result?.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result!.Result!;
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value;
        problemDetails.Title.Should().NotBeNullOrEmpty();
        problemDetails.Detail.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, hasNotReachedLimit: false);
        var preference = CreatePreference(preferenceId);
        var request = CreateRequest(partnerId, preferenceId);
        _partnerRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _customerRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCustomerSet(preference).ToArray());

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        result?.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result!.Result!;
        objectResult.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        objectResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)objectResult.Value;
        problemDetails.Title.Should().NotBeNullOrEmpty();
        problemDetails.Detail.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        // Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId);
        var preference = CreatePreference(preferenceId);
        var request = CreateRequest(partnerId, preferenceId);
        PromoCode? capturedPromoCode = null;
        _partnerRepositoryMock
            .Setup(r => r.GetById(partnerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _customerRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCustomerSet(preference).ToArray());
        _promoCodeRepositoryMock
            .Setup(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Callback<PromoCode, CancellationToken>((pc, _) => capturedPromoCode = pc)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Create(request, CancellationToken.None);

        // Assert
        _promoCodeRepositoryMock.Verify(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()), Times.Once);
        capturedPromoCode.Should().NotBeNull();
        result?.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result!.Result!;
        createdAtActionResult.ActionName.Should().Be(nameof(_sut.GetById));
        var isRouteValuesValid = createdAtActionResult.RouteValues?.ToArray() switch
        {
            [{ Value: Guid pcid }] when pcid == capturedPromoCode.Id => true,
            _ => false
        };

        isRouteValuesValid.Should().BeTrue();
        createdAtActionResult.Value.Should().BeOfType<PromoCodeShortResponse>();
    }

    private PromoCodeCreateRequest CreateRequest(Guid partnerId, Guid preferenceId)
        => new AutoFaker<PromoCodeCreateRequest>()
            .RuleFor(r => r.PartnerId, partnerId)
            .RuleFor(r => r.PreferenceId, preferenceId)
            .RuleFor(r => r.BeginDate, _ => DateTimeOffset.UtcNow)
            .RuleFor(r => r.EndDate, f => f.Date.FutureOffset())
            .Generate();

    private Preference CreatePreference(Guid id)
        => new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => id)
            .RuleFor(p => p.Name, f => f.Lorem.Sentence(4, 2))
            .RuleFor(p => p.Customers, _ => [])
            .Generate();

    private ICollection<Customer> CreateCustomerSet(Preference preference)
        => new AutoFaker<Customer>()
        .RuleFor(c => c.Id, _ => Guid.NewGuid())
        .RuleFor(c => c.Preferences, _ => [preference])
        .RuleFor(c => c.CustomerPromoCodes, _ => [])
        .GenerateBetween(2, 10);

    private Partner CreatePartner(Guid id, bool hasNotCanceledLimit = true, bool hasNotReachedLimit = true)
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

        var createdAt = faker.Date.PastOffset();
        var fakerLimit = new AutoFaker<PartnerPromoCodeLimit>();
        fakerLimit
            .RuleFor(l => l.CreatedAt, _ => createdAt)
            .RuleFor(l => l.EndAt, f => f.Date.FutureOffset())
            .RuleFor(l => l.CanceledAt, f => null)
            .RuleFor(l => l.Limit, f => f.Random.Int(10, 100_000_000))
            .RuleFor(l => l.IssuedCount, (f, l) => f.Random.Int(0, max: l.Limit - 1));
        if (!hasNotReachedLimit)
        {
            fakerLimit
                .RuleFor(l => l.Limit, _ => faker.Random.Int(10, 100_000_000))
                .RuleFor(l => l.IssuedCount, (f, l) => l.Limit + f.Random.Int(0, 3));
        }

        if (!hasNotCanceledLimit)
        {
            fakerLimit
                .RuleFor(l => l.CanceledAt, f => f.Date.BetweenOffset(createdAt, DateTimeOffset.Now));
        }

        var limit = fakerLimit
            .Generate();

        return new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => id)
            .RuleFor(p => p.Name, f => f.Company.CompanyName())
            .RuleFor(p => p.IsActive, _ => true)
            .RuleFor(p => p.Manager, _ => manager)
            .RuleFor(p => p.PartnerLimits, _ => [limit])
            .Generate();
    }
}
