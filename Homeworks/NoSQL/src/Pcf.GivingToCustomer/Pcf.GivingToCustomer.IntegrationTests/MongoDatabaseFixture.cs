using System;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.IntegrationTests.Data;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    public class MongoDatabaseFixture : IDisposable
    {
        private const string ConnectionString = "mongodb://admin:secret@localhost:27017/?authSource=admin";
        private const string DatabaseName = "PromoCodeFactoryGivingToCustomerTest";

        private readonly MongoClient _client;
        private readonly MongoDbTestInitializer _initializer;

        public IMongoDatabase Database { get; }
        public IRepository<Customer> CustomerRepository { get; }
        public IRepository<Preference> PreferenceRepository { get; }

        public MongoDatabaseFixture()
        {
            MongoDbMappings.Register();
            _client = new MongoClient(ConnectionString);
            Database = _client.GetDatabase(DatabaseName);

            _initializer = new MongoDbTestInitializer(Database);
            _initializer.InitializeDb();

            CustomerRepository = new CustomerMongoRepository(Database);
            PreferenceRepository = new MongoRepository<Preference>(Database, "Preferences");
        }

        public void Dispose()
        {
            _initializer.CleanDb();
        }
    }
}
