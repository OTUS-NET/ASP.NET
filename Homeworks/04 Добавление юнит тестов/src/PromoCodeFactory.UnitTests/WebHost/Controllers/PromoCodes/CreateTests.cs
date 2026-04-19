using System.Linq.Expressions;
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
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly Mock<IRepository<PromoCode>> _promoCodeRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferenceRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodeRepositoryMock;
    private readonly PromoCodesController _promoCodesController;

    public CreateTests()
    {
        _partnersRepositoryMock = new Mock<IRepository<Partner>>();
        _promoCodeRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customerRepositoryMock = new Mock<IRepository<Customer>>();
        _preferenceRepositoryMock= new Mock<IRepository<Preference>>();
        _customerPromoCodeRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();

        _promoCodesController = new PromoCodesController(_promoCodeRepositoryMock.Object, _customerRepositoryMock.Object, _customerPromoCodeRepositoryMock.Object, _partnersRepositoryMock.Object, _preferenceRepositoryMock.Object);
    }


    [Fact]
    public async Task Create_WhenPartnerNotFound_ReturnsNotFound()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var request = new PromoCodeCreateRequest("Code", "ServiceInfo", partnerId, DateTime.Now, DateTime.Now, new Guid());
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Partner?)null);

        //Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        //Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Which;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = notFoundResult.Value as ProblemDetails;

        problemDetails!.Title.Should().Be("Partner not found");
        problemDetails.Detail.Should().Be($"Partner with Id {partnerId} not found.");
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true);
        var request = new PromoCodeCreateRequest("CODE", "ServiceInfo", partnerId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30), preferenceId);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        //Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        //Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Which;
        notFoundResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = notFoundResult.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Preference not found");
        problemDetails.Detail.Should().Be($"Preference with Id {preferenceId} not found.");
    }

    [Fact]
    public async Task Create_WhenNoActiveLimit_ReturnsUnprocessableEntity()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true);
        var preference = CreatePreference(preferenceId);
        var request = new PromoCodeCreateRequest("CODE", "ServiceInfo", partnerId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30), preferenceId);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        //Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        //Assert
        var unprocessableResult = result.Result.Should().BeOfType<ObjectResult>().Which;
        unprocessableResult.StatusCode.Should().Be(422);
        unprocessableResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = unprocessableResult.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("No active limit");
        problemDetails.Detail.Should().Be("Partner has no active promo code limit.");
    }

    [Fact]
    public async Task Create_WhenLimitExceeded_ReturnsUnprocessableEntity()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true, limit: 1, issuedCount: 1);
        var preference = CreatePreference(preferenceId);
        var request = new PromoCodeCreateRequest("CODE", "ServiceInfo", partnerId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30), preferenceId);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        //Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        //Assert
        var unprocessableResult = result.Result.Should().BeOfType<ObjectResult>().Which;
        unprocessableResult.StatusCode.Should().Be(422);
        unprocessableResult.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = unprocessableResult.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("Limit exceeded");
        problemDetails.Detail.Should().Be($"Cannot create promo code. Limit would be exceeded (current: 1/1).");
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndIncrementsIssuedCount()
    {
        //Arrange
        var partnerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var partner = CreatePartner(partnerId, true, limit: 100, issuedCount: 0);
        var preference = CreatePreference(preferenceId);
        var request = new PromoCodeCreateRequest("CODE", "ServiceInfo", partnerId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30), preferenceId);
        _partnersRepositoryMock.Setup(x => x.GetById(partnerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(partner);
        _preferenceRepositoryMock.Setup(x => x.GetById(preferenceId, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);
        _customerRepositoryMock.Setup(x => x.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), default))
            .ReturnsAsync(new List<Customer>());

        //Act
        var result = await _promoCodesController.Create(request, CancellationToken.None);

        //Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Which;
        createdResult.Value.Should().BeOfType<PromoCodeShortResponse>();
        var response = createdResult.Value as PromoCodeShortResponse;
        response!.Code.Should().Be("CODE");

        _promoCodeRepositoryMock.Verify(
            x => x.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _partnersRepositoryMock.Verify(
            x => x.Update(It.Is<Partner>(p => p.PartnerLimits.Any(l => l.IssuedCount == 1)), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Partner CreatePartner(
        Guid partnerId,
        bool isActive,
        int limit = 0,
        int issuedCount = 0)
    {
        var partner = new AutoFaker<Partner>()
            .RuleFor(p => p.Id, _ => partnerId)
            .RuleFor(p => p.IsActive, _ => isActive)
            .RuleFor(p => p.PartnerLimits, _ => [])
            .Generate();
        partner.Manager = new AutoFaker<Employee>().Generate();

        if (limit > 0)
        {
            var partnerLimit = new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                Partner = partner,
                Limit = limit,
                IssuedCount = issuedCount,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
                EndAt = DateTimeOffset.UtcNow.AddDays(30)
            };
            partner.PartnerLimits.Add(partnerLimit);
        }

        return partner;
    }

    private static Preference CreatePreference(Guid preferenceId)
    {
        return new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => preferenceId)
            .Generate();
    }
}
