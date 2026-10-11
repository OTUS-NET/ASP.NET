using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Data
{
    public static class MongoDbMappings
    {
        public static void Register()
        {
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(BsonType.String));

            // BaseEntity must be registered first: Id is declared there, not on derived classes.
            // AutoMap() on a derived class triggers base-class registration automatically, but
            // without MapIdMember, so we register BaseEntity explicitly before anything else.
            RegisterBaseEntity();
            RegisterPreference();
            RegisterCustomerPreference();
            RegisterPromoCodeCustomer();
            RegisterCustomer();
            RegisterPromoCode();
        }

        private static void RegisterBaseEntity()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(BaseEntity))) return;
            BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.Id);
                cm.SetIgnoreExtraElements(true);
            });
        }

        private static void RegisterPreference()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Preference))) return;
            BsonClassMap.RegisterClassMap<Preference>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        private static void RegisterCustomerPreference()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(CustomerPreference))) return;
            BsonClassMap.RegisterClassMap<CustomerPreference>(cm =>
            {
                cm.AutoMap();
                // CustomerPreference has no Id of its own (EF composite key),
                // so we don't map _id here — it is always embedded inside Customer documents.
                cm.UnmapMember(x => x.Customer); // back-reference → circular: Customer embeds CustomerPreference
                cm.SetIgnoreExtraElements(true);
            });
        }

        private static void RegisterPromoCodeCustomer()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PromoCodeCustomer))) return;
            BsonClassMap.RegisterClassMap<PromoCodeCustomer>(cm =>
            {
                cm.AutoMap();
                // Id is inherited from BaseEntity — already mapped to _id via RegisterBaseEntity
                cm.UnmapMember(x => x.Customer); // back-reference → Customer embeds PromoCodeCustomer → circular
                // PromoCode IS mapped: stored as a snapshot inside Customer documents so that
                // CustomerResponse can read its fields. In PromoCode collection documents the
                // field is null (set explicitly in PromoCodeMongoRepository), so no circular chain.
                cm.SetIgnoreExtraElements(true);
            });
        }

        private static void RegisterCustomer()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Customer))) return;
            BsonClassMap.RegisterClassMap<Customer>(cm =>
            {
                cm.AutoMap();
                // Id is inherited from BaseEntity — already mapped to _id via RegisterBaseEntity
                cm.UnmapMember(x => x.FullName); // computed read-only property, not stored
                cm.SetIgnoreExtraElements(true);
            });
        }

        private static void RegisterPromoCode()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PromoCode))) return;
            BsonClassMap.RegisterClassMap<PromoCode>(cm =>
            {
                cm.AutoMap();
                // Id is inherited from BaseEntity — already mapped to _id via RegisterBaseEntity
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
