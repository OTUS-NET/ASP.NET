using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Data;

public class MongoDbInitializer : IDbInitializer
{
    private readonly IMongoCollection<Employee> _employeeCollection;

    public MongoDbInitializer(IMongoCollection<Employee> employeeCollection)
    {
        _employeeCollection = employeeCollection;
    }
    
    public void InitializeDb()
    {
        _employeeCollection.DeleteMany(_ => true);
        _employeeCollection.InsertMany(FakeDataFactory.Employees);
    }
}