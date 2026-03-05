using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Mongo
{
    public static class MongoMappings
    {
        private static bool _registered;

        public static void Register()
        {
            if (_registered)
                return;

            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));

            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(nameof(BaseEntity.Id))
                        .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.SetIsRootClass(true);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerPreference)))
            {
                BsonClassMap.RegisterClassMap<CustomerPreference>(cm =>
                {
                    cm.AutoMap();
                    cm.UnmapMember(x => x.Customer);
                    cm.UnmapMember(x => x.Preference);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PromoCodeCustomer)))
            {
                BsonClassMap.RegisterClassMap<PromoCodeCustomer>(cm =>
                {
                    cm.AutoMap();
                    cm.UnmapMember(x => x.PromoCode);
                    cm.UnmapMember(x => x.Customer);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Customer)))
            {
                BsonClassMap.RegisterClassMap<Customer>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Preference)))
            {
                BsonClassMap.RegisterClassMap<Preference>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PromoCode)))
            {
                BsonClassMap.RegisterClassMap<PromoCode>(cm =>
                {
                    cm.AutoMap();
                    cm.UnmapMember(x => x.Preference);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _registered = true;
        }
    }
}

