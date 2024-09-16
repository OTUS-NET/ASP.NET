using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.Administration.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.Administration.DataAccess
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.Connection);
            _database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        }

        public IMongoDatabase Database => _database;
    }
}
