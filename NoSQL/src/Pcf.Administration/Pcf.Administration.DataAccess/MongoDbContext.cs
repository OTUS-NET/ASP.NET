using MongoDB.Driver;
using Pcf.Administration.Core.Domain.Administration;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Employee> Employees => _database.GetCollection<Employee>("Employees");
    public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Roles");
}
