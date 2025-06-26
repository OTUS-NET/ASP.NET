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
        var mongoDatabase = MongoClient.GetDatabase("test_database");
            
        EmployeesCollection = mongoDatabase.GetCollection<Employee>("employees");
        RolesCollection = mongoDatabase.GetCollection<Role>("roles");

        EmployeesCollection.InsertMany(TestDataFactory.Employees);
        RolesCollection.InsertMany(TestDataFactory.Roles);
    }

    public void CleanDb()
    {
        MongoClient.DropDatabase("test_database");
    }
}