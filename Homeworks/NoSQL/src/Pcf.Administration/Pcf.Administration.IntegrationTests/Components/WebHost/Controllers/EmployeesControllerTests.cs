using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using Pcf.Administration.WebHost.Controllers;
using Xunit;

namespace Pcf.Administration.IntegrationTests.Components.WebHost.Controllers
{
    [Collection(MongoDatabaseCollection.DbCollection)]
    public class EmployeesControllerTests: IClassFixture<MongoDatabaseFixture>
    {
        private readonly EmployeesController _employeesController;

        public EmployeesControllerTests(MongoDatabaseFixture mongoDatabaseFixture)
        {
            var employeesRepository = mongoDatabaseFixture.EmployeeRepository;
            var rolesRepository = mongoDatabaseFixture.RoleRepository;

            _employeesController = new EmployeesController(employeesRepository, rolesRepository);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_ExistedEmployee_ExpectedId()
        {
            //Arrange
            var expectedEmployeeId = new ObjectId("000000000000000000000001");

            //Act
            var result = await _employeesController.GetEmployeeByIdAsync("000000000000000000000001");

            //Assert
            result.Value.Id.Should().Be(expectedEmployeeId);
        }
    }
}