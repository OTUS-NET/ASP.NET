using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Data;

namespace Pcf.Administration.DataAccess.Mongo.Data;
public class MongoDbInitializer : IDbInitializer
{
    private readonly MongoDbContext _dataContext;

    public MongoDbInitializer(MongoDbContext dataContext)
    {
        _dataContext = dataContext;
    }

    public void InitializeDb()
    {
        var database = _dataContext.Database;

        database.DropCollection(nameof(Role));
        database.DropCollection(nameof(Employee));

        database.CreateCollection(nameof(Role));
        database.CreateCollection(nameof(Employee));

        var roles = database.GetCollection<Role>(nameof(Role));
        var employees = database.GetCollection<Employee>(nameof(Employee));

        roles.InsertMany(FakeDataFactory.Roles);
        employees.InsertMany(FakeDataFactory.Employees);
    }
}
