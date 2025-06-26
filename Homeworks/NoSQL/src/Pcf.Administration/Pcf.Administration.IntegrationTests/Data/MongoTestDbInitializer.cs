using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.IntegrationTests.Data;

public class MongoTestDbInitializer : IDbInitializer
{
    public IMongoClient MongoClient { get; private set; }
    public IMongoCollection<Employee> EmployeesCollection { get; private set; }
    public IMongoCollection<Role> RolesCollection { get; private set; }

    public void InitializeDb()
    {
        MongoClient = new MongoClient("mongodb://localhost:27017/");
        MongoClient.DropDatabase("test_database");
        var mongoDatabase = MongoClient.GetDatabase("test_database");

        RolesCollection = mongoDatabase.GetCollection<Role>("roles");
        EmployeesCollection = mongoDatabase.GetCollection<Employee>("employees");

        RolesCollection.InsertMany(TestDataFactory.Roles);
        EmployeesCollection.InsertMany(TestDataFactory.Employees);
    }

    public void CleanDb()
    {
        MongoClient.DropDatabase("test_database");
    }
}