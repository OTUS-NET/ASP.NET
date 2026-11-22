using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.WebHost.Controllers;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Exceptions;

namespace Pcf.Administration.UnitTests.WebHost.Controllers.Employees;

public class DeleteTests
{
    private readonly Mock<IRepository<Employee>> _employeesRepositoryMock;
    private readonly EmployeesController _sut;

    public DeleteTests()
    {
        _employeesRepositoryMock = new Mock<IRepository<Employee>>();
        _sut = new EmployeesController(_employeesRepositoryMock.Object, Mock.Of<IRepository<Role>>());
    }

    [Fact]
    public async Task Delete_WhenValidRequest_ReturnsNoContent()
    {
        var employeeId = Guid.NewGuid();

        _employeesRepositoryMock
            .Setup(r => r.Delete(employeeId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Delete(employeeId, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        _employeesRepositoryMock.Verify(
            r => r.Delete(employeeId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenDeleteThrowsEntityNotFoundException_ReturnsNotFound()
    {
        var employeeId = Guid.NewGuid();

        _employeesRepositoryMock
            .Setup(r => r.Delete(employeeId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EntityNotFoundException<Employee>(employeeId));

        var result = await _sut.Delete(employeeId, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }
}
