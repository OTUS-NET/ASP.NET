using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Exceptions;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Controllers;
using Pcf.GivingToCustomer.WebHost.Models.Customers;
using Soenneker.Utils.AutoBogus;

namespace Pcf.GivingToCustomer.UnitTests.WebHost.Controllers.Customers;

public class UpdateTests
{
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly CustomersController _sut;

    public UpdateTests()
    {
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new CustomersController(
            _customersRepositoryMock.Object,
            _preferencesRepositoryMock.Object,
            Mock.Of<IRepository<PromoCode>>());
    }

    [Fact]
    public async Task Update_WhenCustomerNotFound_ReturnsNotFound()
    {
        var customerId = Guid.NewGuid();
        var request = CreateRequest(Guid.NewGuid());

        _customersRepositoryMock
            .Setup(r => r.GetById(customerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _sut.Update(customerId, request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
        _preferencesRepositoryMock.Verify(
            r => r.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenPreferenceNotFound_ReturnsBadRequest()
    {
        var customerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var customer = CreateCustomer(customerId);
        var request = CreateRequest(preferenceId);

        _customersRepositoryMock
            .Setup(r => r.GetById(customerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        var result = await _sut.Update(customerId, request, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        var problemDetails = badRequest.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Invalid preference");

        _customersRepositoryMock.Verify(
            r => r.Update(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenValidRequest_ReturnsOkAndUpdatesCustomer()
    {
        var customerId = Guid.NewGuid();
        var preferenceId = Guid.NewGuid();
        var preference = CreatePreference(preferenceId);
        var customer = CreateCustomer(customerId);
        var request = CreateRequest(preferenceId);

        _customersRepositoryMock
            .Setup(r => r.GetById(customerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Update(customerId, request, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        customer.FirstName.Should().Be(request.FirstName);
        customer.LastName.Should().Be(request.LastName);
        customer.Email.Should().Be(request.Email);
        customer.Preferences.Should().ContainSingle().Which.Should().BeSameAs(preference);

        _customersRepositoryMock.Verify(
            r => r.Update(customer, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        var customerId = Guid.NewGuid();
        var preference = CreatePreference(Guid.NewGuid());
        var customer = CreateCustomer(customerId);
        var request = CreateRequest(preference.Id);

        _customersRepositoryMock
            .Setup(r => r.GetById(customerId, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preference.Id, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.Update(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Customer>(customerId));

        var result = await _sut.Update(customerId, request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private static CustomerUpdateRequest CreateRequest(Guid preferenceId)
    {
        return new CustomerUpdateRequest("Maria", "Petrova", "maria@example.com", [preferenceId]);
    }

    private static Preference CreatePreference(Guid preferenceId)
    {
        return new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => preferenceId)
            .RuleFor(p => p.Customers, _ => [])
            .Generate();
    }

    private static Customer CreateCustomer(Guid customerId)
    {
        return new AutoFaker<Customer>()
            .RuleFor(c => c.Id, _ => customerId)
            .RuleFor(c => c.Preferences, _ => [])
            .RuleFor(c => c.CustomerPromoCodes, _ => [])
            .Generate();
    }
}
