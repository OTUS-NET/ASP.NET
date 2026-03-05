using System.Linq;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Mongo;

namespace Pcf.GivingToCustomer.DataAccess.Data
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

            MongoMappings.Register();

            var preferences = _database.GetCollection<Preference>(_settings.PreferencesCollectionName);
            var customers = _database.GetCollection<Customer>(_settings.CustomersCollectionName);
            var promoCodes = _database.GetCollection<PromoCode>(_settings.PromoCodesCollectionName);

            preferences.InsertMany(FakeDataFactory.Preferences);

            var seededCustomers = FakeDataFactory.Customers;
            foreach (var customer in seededCustomers)
            {
                if (customer.Preferences != null)
                {
                    foreach (var cp in customer.Preferences)
                    {
                        cp.Customer = null;
                        cp.Preference = null;
                    }
                }

                customer.PromoCodes ??= new System.Collections.Generic.List<PromoCodeCustomer>();
            }

            customers.InsertMany(seededCustomers);
        }
    }
}

