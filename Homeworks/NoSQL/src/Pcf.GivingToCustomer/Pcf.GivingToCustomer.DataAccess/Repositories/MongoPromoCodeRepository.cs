using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Mongo;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class MongoPromoCodeRepository : IRepository<PromoCode>
    {
        private readonly IMongoCollection<PromoCode> _promoCodes;
        private readonly IMongoCollection<Customer> _customers;

        public MongoPromoCodeRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
        {
            _promoCodes = database.GetCollection<PromoCode>(settings.Value.PromoCodesCollectionName);
            _customers = database.GetCollection<Customer>(settings.Value.CustomersCollectionName);
        }

        public async Task<IEnumerable<PromoCode>> GetAllAsync()
        {
            return await _promoCodes.Find(FilterDefinition<PromoCode>.Empty).ToListAsync();
        }

        public async Task<PromoCode> GetByIdAsync(Guid id)
        {
            return await _promoCodes.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PromoCode>> GetRangeByIdsAsync(List<Guid> ids)
        {
            return await _promoCodes.Find(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<PromoCode> GetFirstWhere(Expression<Func<PromoCode, bool>> predicate)
        {
            return await _promoCodes.AsQueryable().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<PromoCode>> GetWhere(Expression<Func<PromoCode, bool>> predicate)
        {
            return await _promoCodes.AsQueryable().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(PromoCode entity)
        {
            NormalizeForStorage(entity);
            await _promoCodes.InsertOneAsync(entity);

            var customerIds = (entity.Customers ?? Array.Empty<PromoCodeCustomer>())
                .Select(x => x.CustomerId)
                .Distinct()
                .ToList();

            if (customerIds.Count == 0)
                return;

            foreach (var customerId in customerIds)
            {
                var link = new PromoCodeCustomer
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    PromoCodeId = entity.Id,
                    Customer = null,
                    PromoCode = null
                };

                var update = Builders<Customer>.Update.AddToSet(x => x.PromoCodes, link);
                await _customers.UpdateOneAsync(x => x.Id == customerId, update);
            }
        }

        public async Task UpdateAsync(PromoCode entity)
        {
            NormalizeForStorage(entity);
            await _promoCodes.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        }

        public async Task DeleteAsync(PromoCode entity)
        {
            await _promoCodes.DeleteOneAsync(x => x.Id == entity.Id);
        }

        private static void NormalizeForStorage(PromoCode promoCode)
        {
            if (promoCode == null)
                return;

            promoCode.Preference = null;

            if (promoCode.Customers == null)
                return;

            foreach (var pc in promoCode.Customers)
            {
                if (pc.Id == Guid.Empty)
                    pc.Id = Guid.NewGuid();

                pc.PromoCodeId = promoCode.Id;
                pc.PromoCode = null;
                pc.Customer = null;
            }
        }
    }
}

