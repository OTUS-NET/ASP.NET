using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.WebHost.Controllers;
using Pcf.Administration.WebHost.Models.Employees;
using Pcf.Common.Core.Abstractions.Repositories;
using Soenneker.Utils.AutoBogus;

namespace Pcf.Administration.UnitTests.WebHost.Controllers.Employees;

public class CreateTests
{
    private readonly Mock<IRepository<Employee>> _employeesRepositoryMock;
    private readonly Mock<IRepository<Role>> _rolesRepositoryMock;
    private readonly EmployeesController _sut;

    public CreateTests()
    {
        _employeesRepositoryMock = new Mock<IRepository<Employee>>();
        _rolesRepositoryMock = new Mock<IRepository<Role>>();
        _sut = new EmployeesController(_employeesRepositoryMock.Object, _rolesRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_WhenRoleNotFound_ReturnsBadRequest()
    {
        var roleId = Guid.NewGuid();
        var request = CreateRequest(roleId);

        _rolesRepositoryMock
            .Setup(r => r.GetById(roleId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var result = await _sut.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        badRequest.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = (ProblemDetails)badRequest.Value!;
        problemDetails.Title.Should().Be("Invalid role");

        _employeesRepositoryMock.Verify(
            r => r.Add(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_WhenValidRequest_ReturnsCreatedAndAddsEmployee()
    {
        var roleId = Guid.NewGuid();
        var role = CreateRole(roleId);
        var request = CreateRequest(roleId);

        _rolesRepositoryMock
            .Setup(r => r.GetById(roleId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _employeesRepositoryMock
            .Setup(r => r.Add(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Create(request, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var created = (CreatedAtActionResult)result.Result!;
        created.ActionName.Should().Be(nameof(EmployeesController.GetById));

        var employee = created.Value.Should().BeOfType<Employee>().Subject;
        employee.FirstName.Should().Be(request.FirstName);
        employee.LastName.Should().Be(request.LastName);
        employee.Email.Should().Be(request.Email);
        employee.Role.Should().BeSameAs(role);

        _employeesRepositoryMock.Verify(
            r => r.Add(employee, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static EmployeeCreateRequest CreateRequest(Guid roleId)
    {
        return new EmployeeCreateRequest("Ivan", "Sergeev", "ivan@example.com", roleId);
    }

    private static Role CreateRole(Guid roleId)
    {
        return new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => roleId)
            .Generate();
    }
}
