using System.Collections.Generic;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class CustomerMongoRepository : MongoRepository<Customer>
    {
        public CustomerMongoRepository(IMongoDatabase database) : base(database, "Customers") { }

        // Ensure navigation collections are never null so CustomerResponse doesn't throw.
        // Preferences and PromoCodes start as null on a newly constructed Customer
        // (domain model has no constructor initializer) and may be absent in BSON documents
        // written before the embedded arrays were populated.
        protected override Customer AfterLoad(Customer entity)
        {
            if (entity == null) return null;
            entity.Preferences ??= new List<CustomerPreference>();
            entity.PromoCodes ??= new List<PromoCodeCustomer>();
            return entity;
        }
    }
}
