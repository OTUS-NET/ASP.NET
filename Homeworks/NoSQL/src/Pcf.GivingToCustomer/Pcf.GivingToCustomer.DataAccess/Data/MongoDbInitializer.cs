using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoDbInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public void InitializeDb()
        {
            var preferences = _database.GetCollection<Preference>("Preferences");
            var customers = _database.GetCollection<Customer>("Customers");

            SeedPreferences(preferences);
            CreateIndexes(customers);
            SeedCustomers(customers, preferences);
        }

        private static void SeedPreferences(IMongoCollection<Preference> preferences)
        {
            foreach (var pref in FakeDataFactory.Preferences)
            {
                var filter = Builders<Preference>.Filter.Eq(p => p.Id, pref.Id);
                var exists = preferences.Find(filter).Any();
                if (!exists)
                    preferences.InsertOne(pref);
            }
        }

        private static void CreateIndexes(IMongoCollection<Customer> customers)
        {
            // Multi-key index on the embedded Preferences array — speeds up the
            // "give promo code to customers with preference X" query
            var indexKeys = Builders<Customer>.IndexKeys
                .Ascending("Preferences.PreferenceId");
            customers.Indexes.CreateOne(new CreateIndexModel<Customer>(indexKeys));
        }

        private static void SeedCustomers(IMongoCollection<Customer> customers, IMongoCollection<Preference> preferences)
        {
            var seedCustomerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
            var filter = Builders<Customer>.Filter.Eq(c => c.Id, seedCustomerId);
            if (customers.Find(filter).Any())
                return;

            // Load seeded preferences so we can embed them inside CustomerPreference.
            // CustomerResponse.Preferences reads x.Preference.Name, so the navigation
            // property must be populated before storing the customer document.
            var allPrefs = preferences.Find(FilterDefinition<Preference>.Empty).ToList()
                .ToDictionary(p => p.Id);

            var customer = new Customer
            {
                Id = seedCustomerId,
                Email = "ivan_sergeev@mail.ru",
                FirstName = "Иван",
                LastName = "Петров",
                Preferences = new List<CustomerPreference>
                {
                    BuildCustomerPreference(seedCustomerId,
                        Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"), allPrefs),
                    BuildCustomerPreference(seedCustomerId,
                        Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"), allPrefs)
                },
                PromoCodes = new List<PromoCodeCustomer>()
            };

            customers.InsertOne(customer);
        }

        private static CustomerPreference BuildCustomerPreference(Guid customerId, Guid preferenceId, Dictionary<Guid, Preference> allPrefs)
        {
            return new CustomerPreference
            {
                CustomerId = customerId,
                PreferenceId = preferenceId,
                Preference = allPrefs.TryGetValue(preferenceId, out var pref) ? pref : null
            };
        }
    }
}
