using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Mongo;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class MongoCustomerRepository : IRepository<Customer>
    {
        private readonly IMongoCollection<Customer> _customers;
        private readonly IMongoCollection<Preference> _preferences;
        private readonly IMongoCollection<PromoCode> _promoCodes;

        public MongoCustomerRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
        {
            _customers = database.GetCollection<Customer>(settings.Value.CustomersCollectionName);
            _preferences = database.GetCollection<Preference>(settings.Value.PreferencesCollectionName);
            _promoCodes = database.GetCollection<PromoCode>(settings.Value.PromoCodesCollectionName);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
            await HydrateCustomerAsync(customers);
            return customers;
        }

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            var customer = await _customers.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (customer == null)
                return null;

            await HydrateCustomerAsync(new[] { customer });
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var customers = await _customers.Find(x => ids.Contains(x.Id)).ToListAsync();
            await HydrateCustomerAsync(customers);
            return customers;
        }

        public async Task<Customer> GetFirstWhere(Expression<Func<Customer, bool>> predicate)
        {
            var customers = await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
            await HydratePreferencesAsync(customers);
            var customer = customers.FirstOrDefault(predicate.Compile());
            if (customer == null)
                return null;

            await HydratePromoCodesAsync(new[] { customer });
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetWhere(Expression<Func<Customer, bool>> predicate)
        {
            var customers = await _customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
            await HydratePreferencesAsync(customers);

            var filtered = customers.Where(predicate.Compile()).ToList();
            return filtered;
        }

        public async Task AddAsync(Customer entity)
        {
            NormalizeForStorage(entity);
            await _customers.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Customer entity)
        {
            NormalizeForStorage(entity);
            await _customers.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        }

        public async Task DeleteAsync(Customer entity)
        {
            await _customers.DeleteOneAsync(x => x.Id == entity.Id);
        }

        private static void NormalizeForStorage(Customer customer)
        {
            if (customer == null)
                return;

            if (customer.Preferences != null)
            {
                foreach (var cp in customer.Preferences)
                {
                    cp.CustomerId = customer.Id;
                    cp.Customer = null;
                    cp.Preference = null;
                }
            }

            if (customer.PromoCodes != null)
            {
                foreach (var pc in customer.PromoCodes)
                {
                    pc.CustomerId = customer.Id;
                    pc.Customer = null;
                    pc.PromoCode = null;
                    if (pc.Id == Guid.Empty)
                        pc.Id = Guid.NewGuid();
                }
            }
        }

        private async Task HydrateCustomerAsync(IEnumerable<Customer> customers)
        {
            var list = customers as IList<Customer> ?? customers.ToList();
            if (list.Count == 0)
                return;

            await HydratePreferencesAsync(list);
            await HydratePromoCodesAsync(list);
        }

        private async Task HydratePreferencesAsync(IEnumerable<Customer> customers)
        {
            var list = customers as IList<Customer> ?? customers.ToList();
            var preferenceIds = list
                .SelectMany(x => x.Preferences ?? Array.Empty<CustomerPreference>())
                .Select(x => x.PreferenceId)
                .Distinct()
                .ToList();

            if (preferenceIds.Count == 0)
                return;

            var preferences = await _preferences.Find(x => preferenceIds.Contains(x.Id)).ToListAsync();
            var byId = preferences.ToDictionary(x => x.Id, x => x);

            foreach (var customer in list)
            {
                if (customer.Preferences == null)
                    continue;

                foreach (var cp in customer.Preferences)
                {
                    if (byId.TryGetValue(cp.PreferenceId, out var preference))
                        cp.Preference = preference;
                }
            }
        }

        private async Task HydratePromoCodesAsync(IEnumerable<Customer> customers)
        {
            var list = customers as IList<Customer> ?? customers.ToList();
            var promoCodeIds = list
                .SelectMany(x => x.PromoCodes ?? Array.Empty<PromoCodeCustomer>())
                .Select(x => x.PromoCodeId)
                .Distinct()
                .ToList();

            if (promoCodeIds.Count == 0)
                return;

            var promoCodes = await _promoCodes.Find(x => promoCodeIds.Contains(x.Id)).ToListAsync();
            var byId = promoCodes.ToDictionary(x => x.Id, x => x);

            foreach (var customer in list)
            {
                if (customer.PromoCodes == null)
                    continue;

                foreach (var pc in customer.PromoCodes)
                {
                    if (byId.TryGetValue(pc.PromoCodeId, out var promoCode))
                        pc.PromoCode = promoCode;
                }
            }
        }
    }
}

