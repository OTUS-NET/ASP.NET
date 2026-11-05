using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Controllers;
using Pcf.GivingToCustomer.WebHost.Models.PromoCodes;
using Soenneker.Utils.AutoBogus;

namespace Pcf.GivingToCustomer.UnitTests.WebHost.Controllers.PromoCodes;

public class ApplyTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly PromoCodesController _sut;

    public ApplyTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _sut = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            _customersRepositoryMock.Object,
            Mock.Of<IRepository<Preference>>());
    }

    [Fact]
    public async Task Apply_WhenPromoCodeNotFound_ReturnsNotFound()
    {
        var promoCodeId = Guid.NewGuid();
        var request = new PromoCodeApplyRequest(Guid.NewGuid());

        _promoCodesRepositoryMock
            .Setup(r => r.GetById(promoCodeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PromoCode?)null);

        var result = await _sut.Apply(promoCodeId, request, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
        _customersRepositoryMock.Verify(
            r => r.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Apply_WhenCustomerWasNotIssuedPromoCode_ReturnsNotFound()
    {
        var promoCodeId = Guid.NewGuid();
        var promoCode = CreatePromoCode(promoCodeId);
        var request = new PromoCodeApplyRequest(Guid.NewGuid());

        _promoCodesRepositoryMock
            .Setup(r => r.GetById(promoCodeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promoCode);

        _customersRepositoryMock
            .Setup(r => r.GetById(request.CustomerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCustomer(request.CustomerId));

        var result = await _sut.Apply(promoCodeId, request, CancellationToken.None);

        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = (NotFoundObjectResult)result;
        var problemDetails = notFound.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Customer promo code not found");
    }

    [Fact]
    public async Task Apply_WhenValidRequest_ReturnsNoContentAndSetsAppliedAt()
    {
        var promoCodeId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var promoCode = CreatePromoCode(promoCodeId);
        var customerPromoCode = new CustomerPromoCode
        {
            Id = Guid.NewGuid(),
            PromoCodeId = promoCodeId,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        var customer = CreateCustomer(customerId);
        customer.CustomerPromoCodes.Add(customerPromoCode);
        var request = new PromoCodeApplyRequest(customerId);

        _promoCodesRepositoryMock
            .Setup(r => r.GetById(promoCodeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promoCode);

        _customersRepositoryMock
            .Setup(r => r.GetById(customerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _customersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Apply(promoCodeId, request, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        customerPromoCode.AppliedAt.Should().NotBeNull();
        _customersRepositoryMock.Verify(
            r => r.Update(customer, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static PromoCode CreatePromoCode(Guid promoCodeId)
    {
        return new AutoFaker<PromoCode>()
            .RuleFor(p => p.Id, _ => promoCodeId)
            .RuleFor(p => p.PartnerId, _ => Guid.NewGuid())
            .RuleFor(p => p.PreferenceId, _ => Guid.NewGuid())
            .Generate();
    }

    private static Customer CreateCustomer(Guid customerId)
    {
        return new Customer
        {
            Id = customerId,
            FirstName = "Ivan",
            LastName = "Ivanov",
            Email = "ivan@example.com"
        };
    }
}
