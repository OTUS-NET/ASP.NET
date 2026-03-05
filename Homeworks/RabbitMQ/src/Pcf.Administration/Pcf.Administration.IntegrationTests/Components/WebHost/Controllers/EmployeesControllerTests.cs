using System;
using System.Threading.Tasks;
using FluentAssertions;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Repositories;
using Pcf.Administration.WebHost.Controllers;
using Xunit;

namespace Pcf.Administration.IntegrationTests.Components.WebHost.Controllers
{
    [Collection(EfDatabaseCollection.DbCollection)]
    public class EmployeesControllerTests : IClassFixture<EfDatabaseFixture>
    {
        private EfRepository<Employee> _employeesRepository;
        private EmployeesController _employeesController;

        public EmployeesControllerTests(EfDatabaseFixture efDatabaseFixture)
        {
            _employeesRepository = new EfRepository<Employee>(efDatabaseFixture.DbContext);
            _employeesController = new EmployeesController(_employeesRepository, new FakeAppliedPromocodesService(_employeesRepository));
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

        private class FakeAppliedPromocodesService : IAppliedPromocodesService
        {
            private readonly EfRepository<Employee> _employeesRepository;

            public FakeAppliedPromocodesService(EfRepository<Employee> employeesRepository)
            {
                _employeesRepository = employeesRepository;
            }

            public async Task<bool> IncrementAppliedPromocodesAsync(Guid employeeId, int incrementBy = 1)
            {
                if (incrementBy <= 0)
                    return false;

                var employee = await _employeesRepository.GetByIdAsync(employeeId);
                if (employee == null)
                    return false;

                employee.AppliedPromocodesCount += incrementBy;
                await _employeesRepository.UpdateAsync(employee);

                return true;
            }
        }
    }
}