using FluentAssertions;
using Moq;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.Core.Services;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Administration.WebHost.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pcf.Administration.IntegrationTests.Components.WebHost.Controllers
{
    [Collection(EfDatabaseCollection.DbCollection)]
    public class EmployeesControllerTests : IClassFixture<EfDatabaseFixture>
    {
        private readonly EfRepository<Employee> _employeesRepository;
        private readonly EmployeesController _employeesController;

        public EmployeesControllerTests(EfDatabaseFixture efDatabaseFixture)
        {
            _employeesRepository = new EfRepository<Employee>(efDatabaseFixture.DbContext);
            var adminServiceMock = new Mock<IAdministrationService>();
            _employeesController = new EmployeesController(_employeesRepository, adminServiceMock.Object);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ExistedEmployee_ExpectedId()
        {
            //Arrange
            var expectedEmployeeId = Guid.Parse("451533d5-d8d5-4a11-9c7b-eb9f14e1a32f");

            //Act
            var result = await _employeesController.GetEmployeeByIdAsync(expectedEmployeeId);

            //Assert
            result.Value.Id.Should().Be(expectedEmployeeId);
        }
    }
}