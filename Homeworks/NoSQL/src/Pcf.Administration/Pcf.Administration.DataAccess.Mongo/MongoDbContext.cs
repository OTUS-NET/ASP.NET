using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Pcf.Administration.DataAccess.Mongo;
public class MongoDbContext
{
    public IMongoDatabase Database { get; }

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var client = new MongoClient(options.Value.Connection);
        Database = client.GetDatabase(options.Value.DatabaseName);
    }
}
