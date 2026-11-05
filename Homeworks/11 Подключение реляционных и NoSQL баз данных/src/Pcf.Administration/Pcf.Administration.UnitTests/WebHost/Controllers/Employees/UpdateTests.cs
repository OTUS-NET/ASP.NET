using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.WebHost.Controllers;
using Pcf.Administration.WebHost.Models.Employees;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Exceptions;
using Soenneker.Utils.AutoBogus;

namespace Pcf.Administration.UnitTests.WebHost.Controllers.Employees;

public class UpdateTests
{
    private readonly Mock<IRepository<Employee>> _employeesRepositoryMock;
    private readonly Mock<IRepository<Role>> _rolesRepositoryMock;
    private readonly EmployeesController _sut;

    public UpdateTests()
    {
        _employeesRepositoryMock = new Mock<IRepository<Employee>>();
        _rolesRepositoryMock = new Mock<IRepository<Role>>();
        _sut = new EmployeesController(_employeesRepositoryMock.Object, _rolesRepositoryMock.Object);
    }

    [Fact]
    public async Task Update_WhenEmployeeNotFound_ReturnsNotFound()
    {
        var employeeId = Guid.NewGuid();
        var request = CreateRequest(Guid.NewGuid());

        _employeesRepositoryMock
            .Setup(r => r.GetById(employeeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var result = await _sut.Update(employeeId, request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
        _rolesRepositoryMock.Verify(
            r => r.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenRoleNotFound_ReturnsBadRequest()
    {
        var employeeId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var employee = CreateEmployee(employeeId, CreateRole(Guid.NewGuid()));
        var request = CreateRequest(roleId);

        _employeesRepositoryMock
            .Setup(r => r.GetById(employeeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _rolesRepositoryMock
            .Setup(r => r.GetById(roleId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var result = await _sut.Update(employeeId, request, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result!;
        var problemDetails = badRequest.Value.Should().BeOfType<ProblemDetails>().Subject;
        problemDetails.Title.Should().Be("Invalid role");

        _employeesRepositoryMock.Verify(
            r => r.Update(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenValidRequest_ReturnsOkAndUpdatesEmployee()
    {
        var employeeId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var role = CreateRole(roleId);
        var employee = CreateEmployee(employeeId, CreateRole(Guid.NewGuid()));
        var request = CreateRequest(roleId);

        _employeesRepositoryMock
            .Setup(r => r.GetById(employeeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _rolesRepositoryMock
            .Setup(r => r.GetById(roleId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _employeesRepositoryMock
            .Setup(r => r.Update(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Update(employeeId, request, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        employee.FirstName.Should().Be(request.FirstName);
        employee.LastName.Should().Be(request.LastName);
        employee.Email.Should().Be(request.Email);
        employee.Role.Should().BeSameAs(role);

        _employeesRepositoryMock.Verify(
            r => r.Update(employee, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenUpdateThrowsEntityNotFoundException_ReturnsNotFound()
    {
        var employeeId = Guid.NewGuid();
        var role = CreateRole(Guid.NewGuid());
        var employee = CreateEmployee(employeeId, role);
        var request = CreateRequest(role.Id);

        _employeesRepositoryMock
            .Setup(r => r.GetById(employeeId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _rolesRepositoryMock
            .Setup(r => r.GetById(role.Id, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _employeesRepositoryMock
            .Setup(r => r.Update(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Employee>(employeeId));

        var result = await _sut.Update(employeeId, request, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private static EmployeeUpdateRequest CreateRequest(Guid roleId)
    {
        return new EmployeeUpdateRequest("Petr", "Andreev", "andreev@example.com", roleId);
    }

    private static Role CreateRole(Guid roleId)
    {
        return new AutoFaker<Role>()
            .RuleFor(r => r.Id, _ => roleId)
            .Generate();
    }

    private static Employee CreateEmployee(Guid employeeId, Role role)
    {
        return new AutoFaker<Employee>()
            .RuleFor(e => e.Id, _ => employeeId)
            .RuleFor(e => e.Role, role)
            .Generate();
    }
}
