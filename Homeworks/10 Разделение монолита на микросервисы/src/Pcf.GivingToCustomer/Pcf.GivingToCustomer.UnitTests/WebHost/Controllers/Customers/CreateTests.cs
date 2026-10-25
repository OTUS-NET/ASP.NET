using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Controllers;
using Pcf.GivingToCustomer.WebHost.Models.Customers;
using Soenneker.Utils.AutoBogus;

namespace Pcf.GivingToCustomer.UnitTests.WebHost.Controllers.Customers;

public class CreateTests
{
    private readonly Mock<IRepository<Customer>> _customersRepositoryMock;
    private readonly Mock<IRepository<Preference>> _preferencesRepositoryMock;
    private readonly CustomersController _sut;

    public CreateTests()
    {
        _customersRepositoryMock = new Mock<IRepository<Customer>>();
        _preferencesRepositoryMock = new Mock<IRepository<Preference>>();
        _sut = new CustomersController(
            _customersRepositoryMock.Object,
            _preferencesRepositoryMock.Object,
            Mock.Of<IRepository<PromoCode>>());
    }

    [Fact]
    public async Task Create_WhenPreferenceNotFound_ReturnsBadRequest()
    {
        var preferenceId = Guid.NewGuid();
        var request = CreateRequest(preferenceId);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Preference?)null);

        var result = await _sut.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        var problemDetails = badRequest.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Invalid preference");

        _customersRepositoryMock.Verify(
            r => r.Add(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndAddsCustomer()
    {
        var preferenceId = Guid.NewGuid();
        var preference = CreatePreference(preferenceId);
        var request = CreateRequest(preferenceId);

        _preferencesRepositoryMock
            .Setup(r => r.GetById(preferenceId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(preference);

        _customersRepositoryMock
            .Setup(r => r.Add(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = (CreatedAtActionResult)result.Result!;
        created.ActionName.Should().Be(nameof(CustomersController.GetById));

        var response = created.Value.Should().BeOfType<CustomerShortResponse>().Subject;
        response.FirstName.Should().Be(request.FirstName);
        response.Email.Should().Be(request.Email);
        response.Preferences.Should().ContainSingle(p => p.Id == preferenceId);

        _customersRepositoryMock.Verify(
            r => r.Add(It.Is<Customer>(c =>
                c.FirstName == request.FirstName &&
                c.LastName == request.LastName &&
                c.Email == request.Email &&
                c.Preferences.Single() == preference),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CustomerCreateRequest CreateRequest(Guid preferenceId)
    {
        return new CustomerCreateRequest("Ivan", "Ivanov", "ivan@example.com", [preferenceId]);
    }

    private static Preference CreatePreference(Guid preferenceId)
    {
        return new AutoFaker<Preference>()
            .RuleFor(p => p.Id, _ => preferenceId)
            .RuleFor(p => p.Customers, _ => [])
            .Generate();
    }
}
