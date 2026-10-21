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

public class ApplyTests
{
    private readonly Mock<IRepository<PromoCode>> _promoCodesRepositoryMock;
    private readonly Mock<IRepository<CustomerPromoCode>> _customerPromoCodesRepositoryMock;
    private readonly PromoCodesController _sut;

    public ApplyTests()
    {
        _promoCodesRepositoryMock = new Mock<IRepository<PromoCode>>();
        _customerPromoCodesRepositoryMock = new Mock<IRepository<CustomerPromoCode>>();
        _sut = new PromoCodesController(
            _promoCodesRepositoryMock.Object,
            Mock.Of<IRepository<Customer>>(),
            _customerPromoCodesRepositoryMock.Object,
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
        _customerPromoCodesRepositoryMock.Verify(
            r => r.GetWhere(It.IsAny<Expression<Func<CustomerPromoCode, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
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

        _customerPromoCodesRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<CustomerPromoCode, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CustomerPromoCode>());

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
            CustomerId = customerId,
            PromoCodeId = promoCodeId,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        var request = new PromoCodeApplyRequest(customerId);

        _promoCodesRepositoryMock
            .Setup(r => r.GetById(promoCodeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(promoCode);

        _customerPromoCodesRepositoryMock
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<CustomerPromoCode, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([customerPromoCode]);

        _customerPromoCodesRepositoryMock
            .Setup(r => r.Update(It.IsAny<CustomerPromoCode>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Apply(promoCodeId, request, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        customerPromoCode.AppliedAt.Should().NotBeNull();
        _customerPromoCodesRepositoryMock.Verify(
            r => r.Update(customerPromoCode, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static PromoCode CreatePromoCode(Guid promoCodeId)
    {
        var preference = new AutoFaker<Preference>()
            .RuleFor(p => p.Customers, _ => [])
            .Generate();

        return new AutoFaker<PromoCode>()
            .RuleFor(p => p.Id, _ => promoCodeId)
            .RuleFor(p => p.Preference, preference)
            .RuleFor(p => p.PartnerId, _ => Guid.NewGuid())
            .RuleFor(p => p.CustomerPromoCodes, _ => [])
            .Generate();
    }
}
