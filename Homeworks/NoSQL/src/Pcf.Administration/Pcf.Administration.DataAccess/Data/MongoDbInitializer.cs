using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Data;

public class MongoDbInitializer : IDbInitializer
{
    private readonly IMongoCollection<Employee> _employeeCollection;
    private readonly IMongoCollection<Role> _rolesCollection;

    public MongoDbInitializer(IMongoCollection<Employee> employeeCollection, IMongoCollection<Role> rolesCollection)
    {
        _employeeCollection = employeeCollection;
        _rolesCollection = rolesCollection;
    }
    
    public void InitializeDb()
    {
        _rolesCollection.DeleteMany(_ => true);
        _employeeCollection.DeleteMany(_ => true);
        
        _rolesCollection.InsertMany(FakeDataFactory.Roles);
        _employeeCollection.InsertMany(FakeDataFactory.Employees);
    }
}