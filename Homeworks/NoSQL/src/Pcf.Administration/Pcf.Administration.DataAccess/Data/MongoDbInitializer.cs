using System.Linq;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Mongo;

namespace Pcf.Administration.DataAccess.Data
{
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbInitializer(IMongoClient client, IMongoDatabase database, IOptions<MongoDbSettings> settings)
        {
            _client = client;
            _database = database;
            _settings = settings.Value;
        }

        public void InitializeDb()
        {
            _client.DropDatabase(_settings.DatabaseName);

            var roles = _database.GetCollection<Role>(_settings.RolesCollectionName);
            var employees = _database.GetCollection<Employee>(_settings.EmployeesCollectionName);

            MongoMappings.Register();

            var seededRoles = FakeDataFactory.Roles;
            roles.InsertMany(seededRoles);

            var seededEmployees = FakeDataFactory.Employees;
            foreach (var employee in seededEmployees)
            {
                employee.RoleId = employee.Role?.Id ?? employee.RoleId;
                employee.Role = null;
            }

            employees.InsertMany(seededEmployees);
        }
    }
}

