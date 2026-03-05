using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain;
using Pcf.Administration.Core.Domain.Administration;

namespace Pcf.Administration.DataAccess.Mongo
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

            if (!BsonClassMap.IsClassMapRegistered(typeof(Role)))
            {
                BsonClassMap.RegisterClassMap<Role>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Employee)))
            {
                BsonClassMap.RegisterClassMap<Employee>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(nameof(Employee.RoleId))
                        .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    cm.UnmapMember(x => x.Role);
                    cm.SetIgnoreExtraElements(true);
                });
            }

            _registered = true;
        }
    }
}

