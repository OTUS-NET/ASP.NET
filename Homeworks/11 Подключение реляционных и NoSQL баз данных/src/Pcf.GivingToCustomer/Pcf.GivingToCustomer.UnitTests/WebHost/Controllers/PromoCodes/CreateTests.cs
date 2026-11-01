using System.Linq.Expressions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Controllers;
using Pcf.GivingToCustomer.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;

namespace Pcf.GivingToCustomer.UnitTests.WebHost.Controllers.PromoCodes;

public class CreateTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly PromoCodesController _sut;

    public CreateTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            Mock.Of<IRepository<CustomerPromoCode>>(),
            _preferencesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsNotFound()
    {
        var request = CreateRequest();

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        var result = await _sut.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = (NotFoundObjectResult)result.Result!;
        var problemDetails = notFound.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Preference not found");

        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenValidRequest_GivesPromoCodeToCustomersWithPreference()
    {
        var request = CreateRequest();
        var preference = CreatePreference(request.PreferenceId);
        var customers = CreateCustomers(2);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(request.PreferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        _promoCodesRepositoryMock
            .Setup(r => r.Add(It.IsAny<PromoCode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = (CreatedAtActionResult)result.Result!;
        created.ActionName.Should().Be(nameof(PromoCodesController.GetById));
        created.RouteValues!["id"].Should().Be(request.Id);

        _promoCodesRepositoryMock.Verify(
            r => r.Add(It.Is<PromoCode>(pc =>
                pc.Id == request.Id &&
                pc.Code == request.Code &&
                pc.ServiceInfo == request.ServiceInfo &&
                pc.PartnerId == request.PartnerId &&
                pc.Preference == preference &&
                pc.CustomerPromoCodes.Count == customers.Count &&
                pc.CustomerPromoCodes.All(cpc => cpc.PromoCodeId == request.Id) &&
                customers.All(c => pc.CustomerPromoCodes.Any(cpc => cpc.CustomerId == c.Id))),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CreatePromoCodeRequest CreateRequest()
    {
        return new CreatePromoCodeRequest(
            Guid.NewGuid(),
            "PROMO-TEST",
            "Test service info",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(30),
            Guid.NewGuid(),
            Guid.NewGuid());
    }

    private static Preference CreatePreference(Guid preferenceId)
    {
        return new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => preferenceId)
            .RuleFor(p => p.Customers, _ => [])
            .Generate();
    }

    private static IReadOnlyCollection<Customer> CreateCustomers(int count)
    {
        return new AutoFaker<Customer>()
            .RuleFor(c => c.Id, _ => Guid.NewGuid())
            .RuleFor(c => c.Preferences, _ => [])
            .RuleFor(c => c.CustomerPromoCodes, _ => [])
            .Generate(count);
    }
}
