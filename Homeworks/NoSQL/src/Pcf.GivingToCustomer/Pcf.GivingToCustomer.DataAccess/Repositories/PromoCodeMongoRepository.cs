using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class PromoCodeMongoRepository : MongoRepository<PromoCode>
    {
        private readonly IMongoCollection<Customer> _customers;

        public PromoCodeMongoRepository(IMongoDatabase database) : base(database, "PromoCodes")
        {
            _customers = database.GetCollection<Customer>("Customers");
        }

        public override async Task AddAsync(PromoCode entity)
        {
            // Build PromoCode document for the PromoCodes collection.
            // PromoCodeCustomer items stored here carry only scalar IDs —
            // the PromoCode back-reference is set to null so the serializer
            // never encounters the circular PromoCode → Customers → PromoCode chain.
            var promoCodeDoc = BuildPromoCodeDocument(entity);
            await Collection.InsertOneAsync(promoCodeDoc);

            if (entity.Customers == null || !entity.Customers.Any())
                return;

            // A lightweight PromoCode snapshot stored inside each Customer document.
            // Customers list is intentionally null here to break the circular reference.
            var promoCodeSnapshot = BuildPromoCodeSnapshot(entity);

            foreach (var pc in entity.Customers)
            {
                var pcDoc = new PromoCodeCustomer
                {
                    Id = pc.Id == Guid.Empty ? Guid.NewGuid() : pc.Id,
                    PromoCodeId = entity.Id,
                    CustomerId = pc.CustomerId,
                    PromoCode = promoCodeSnapshot
                    // Customer = null — back-reference, already excluded by BsonClassMap
                };

                var filter = Builders<Customer>.Filter.Eq(c => c.Id, pc.CustomerId);
                var update = Builders<Customer>.Update.Push(c => c.PromoCodes, pcDoc);
                await _customers.UpdateOneAsync(filter, update);
            }
        }

        private static PromoCode BuildPromoCodeDocument(PromoCode entity)
        {
            return new PromoCode
            {
                Id = entity.Id,
                Code = entity.Code,
                ServiceInfo = entity.ServiceInfo,
                BeginDate = entity.BeginDate,
                EndDate = entity.EndDate,
                PartnerId = entity.PartnerId,
                Preference = entity.Preference,
                PreferenceId = entity.PreferenceId,
                Customers = entity.Customers?
                    .Select(c => new PromoCodeCustomer
                    {
                        Id = c.Id == Guid.Empty ? Guid.NewGuid() : c.Id,
                        PromoCodeId = entity.Id,
                        CustomerId = c.CustomerId
                        // PromoCode = null — avoids circular serialization
                    })
                    .ToList()
            };
        }

        private static PromoCode BuildPromoCodeSnapshot(PromoCode entity)
        {
            return new PromoCode
            {
                Id = entity.Id,
                Code = entity.Code,
                ServiceInfo = entity.ServiceInfo,
                BeginDate = entity.BeginDate,
                EndDate = entity.EndDate,
                PartnerId = entity.PartnerId,
                Preference = entity.Preference,
                PreferenceId = entity.PreferenceId,
                Customers = null // no back-reference into the per-customer embedded document
            };
        }
    }
}
