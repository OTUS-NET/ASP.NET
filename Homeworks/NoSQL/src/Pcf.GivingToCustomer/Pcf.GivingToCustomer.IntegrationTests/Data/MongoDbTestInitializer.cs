using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Data;

namespace Pcf.GivingToCustomer.IntegrationTests.Data
{
    public class MongoDbTestInitializer : IDbInitializer
    {
        private readonly IMongoDatabase _database;

        public MongoDbTestInitializer(IMongoDatabase database)
        {
            _database = database;
        }

        public void InitializeDb()
        {
            // Drop for test isolation — each fixture lifecycle starts clean
            _database.DropCollection("Preferences");
            _database.DropCollection("Customers");
            _database.DropCollection("PromoCodes");

            var preferences = _database.GetCollection<Preference>("Preferences");
            var customers = _database.GetCollection<Customer>("Customers");

            preferences.InsertMany(TestDataFactory.Preferences);

            var allPrefs = TestDataFactory.Preferences.ToDictionary(p => p.Id);
            customers.InsertOne(BuildSeedCustomer(allPrefs));
        }

        public void CleanDb()
        {
            _database.DropCollection("Preferences");
            _database.DropCollection("Customers");
            _database.DropCollection("PromoCodes");
        }

        private static Customer BuildSeedCustomer(Dictionary<Guid, Preference> allPrefs)
        {
            var customerId = Guid.Parse("a6c8c6b1-4349-45b0-ab31-244740aaf0f0");
            return new Customer
            {
                Id = customerId,
                Email = "ivan_sergeev@mail.ru",
                FirstName = "Иван",
                LastName = "Петров",
                Preferences = new List<CustomerPreference>
                {
                    BuildPref(customerId, Guid.Parse("76324c47-68d2-472d-abb8-33cfa8cc0c84"), allPrefs),
                    BuildPref(customerId, Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c"), allPrefs)
                },
                PromoCodes = new List<PromoCodeCustomer>()
            };
        }

        private static CustomerPreference BuildPref(Guid customerId, Guid preferenceId, Dictionary<Guid, Preference> allPrefs)
        {
            return new CustomerPreference
            {
                CustomerId = customerId,
                PreferenceId = preferenceId,
                Preference = allPrefs.TryGetValue(preferenceId, out var p) ? p : null
            };
        }
    }
}
