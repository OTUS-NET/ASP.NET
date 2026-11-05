using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Exceptions;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Controllers;

namespace Pcf.GivingToCustomer.UnitTests.WebHost.Controllers.Customers;

public class DeleteTests
{
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly CustomersController _sut;

    public DeleteTests()
    {
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _sut = new CustomersController(
            _customersRepositoryMock.Object,
            Mock.Of<IRepository<Preference>>(),
            Mock.Of<IRepository<PromoCode>>());
    }

    [Fact]
    public async Task Delete_WhenValidRequest_ReturnsNoContent()
    {
        var customerId = Guid.NewGuid();

        _customersRepositoryMock
            .Setup(r => r.Delete(customerId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Delete(customerId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        _customersRepositoryMock.Verify(
            r => r.Delete(customerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteThrowsEntityNotFoundException_ReturnsNotFound()
    {
        var customerId = Guid.NewGuid();

        _customersRepositoryMock
            .Setup(r => r.Delete(customerId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Customer>(customerId));

        var result = await _sut.Delete(customerId, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }
}
