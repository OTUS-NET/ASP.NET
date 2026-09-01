using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess
{
    /// <summary>
    /// Контекст работы с MongoDB для микросервиса GivingToCustomer.
    /// Создание клиента, получение базы данных и настройку сериализации доменных сущностей.
    /// </summary>
    public class MongoContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly string _databaseName;

        /// <summary>
        /// Стат. конструктор выполняется один раз для процесса приложения.
        /// Здесь регистрируются правила сериализации Guid и доменных классов до первого обращения к MongoDB.
        /// </summary>
        static MongoContext()
        {
            RegisterSerializers();
            RegisterClassMaps();
        }

        /// <summary>
        /// Создаёт подключение к MongoDB по настройкам приложения и выбирает рабочую базу данных.
        /// </summary>
        /// <param name="settings">Настройки строки подключения и имени базы данных.</param>
        public MongoContext(MongoDbSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _databaseName = settings.DatabaseName;
            _database = _client.GetDatabase(settings.DatabaseName);
        }

        /// <summary>
        /// Mongo-клиент. Нужен репозиториям и инициализатору, например для удаления тестовой или рабочей базы.
        /// </summary>
        public IMongoClient Client => _client;

        /// <summary>
        /// Рабочая база данных микросервиса.
        /// </summary>
        public IMongoDatabase Database => _database;

        /// <summary>
        /// Имя рабочей базы данных.
        /// </summary>
        public string DatabaseName => _databaseName;

        /// <summary>
        /// Возвращает Mongo-коллекцию для указанной доменной сущности.
        /// Название коллекции определяется централизованно через GetCollectionName.
        /// </summary>
        /// <typeparam name="T">Тип доменной сущности.</typeparam>
        /// <returns>Коллекция MongoDB для чтения и записи сущностей указанного типа.</returns>
        public IMongoCollection<T> GetCollection<T>()
            where T : BaseEntity
        {
            return _database.GetCollection<T>(GetCollectionName<T>());
        }

        /// <summary>
        /// Определяет имя Mongo-коллекции по типу доменной сущности.
        /// Такой маппинг заменяет DbSet из Entity Framework первоначального проекта.
        /// </summary>
        /// <typeparam name="T">Тип доменной сущности.</typeparam>
        /// <returns>Имя коллекции MongoDB.</returns>
        /// <exception cref="NotSupportedException">Возникает, если для типа сущности коллекция не настроена.</exception>
        public static string GetCollectionName<T>()
            where T : BaseEntity
        {
            if (typeof(T) == typeof(Customer))
                return "customers";

            if (typeof(T) == typeof(Preference))
                return "preferences";

            if (typeof(T) == typeof(PromoCode))
                return "promoCodes";

            throw new NotSupportedException($"Collection for type {typeof(T).Name} is not configured.");
        }

        /// <summary>
        /// Регистрирует сериализацию Guid в стандартном формате MongoDB.
        /// Это нужно, чтобы Guid корректно сохранялись и искались по Id в разных версиях Mongo.
        /// </summary>
        private static void RegisterSerializers()
        {
            try
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }
            catch (BsonSerializationException)
            {
            }
        }

        /// <summary>
        /// Настраивает сериализацию доменных классов.
        /// Навигационные свойства, которые раньше обслуживались EF, не сохраняются как вложенные циклические объекты.
        /// </summary>
        private static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Customer)))
            {
                BsonClassMap.RegisterClassMap<Customer>(map =>
                {
                    map.AutoMap();
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerPreference)))
            {
                BsonClassMap.RegisterClassMap<CustomerPreference>(map =>
                {
                    map.AutoMap();
                    map.UnmapMember(x => x.Customer);
                    map.UnmapMember(x => x.Preference);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(PromoCodeCustomer)))
            {
                BsonClassMap.RegisterClassMap<PromoCodeCustomer>(map =>
                {
                    map.AutoMap();
                    map.UnmapMember(x => x.Customer);
                    map.UnmapMember(x => x.PromoCode);
                });
            }
        }
    }
}
