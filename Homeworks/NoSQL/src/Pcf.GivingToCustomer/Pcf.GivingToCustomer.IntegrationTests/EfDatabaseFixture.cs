using System;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.IntegrationTests.Data;

namespace Pcf.GivingToCustomer.IntegrationTests
{
    /// <summary>
    /// Fixture тестовой MongoDB-базы для component-тестов.
    /// Имя сохранено прежним, чтобы не переименовывать коллекции и существующие тестовые классы.
    /// </summary>
    public class EfDatabaseFixture: IDisposable
    {
        private readonly EfTestDbInitializer _efTestDbInitializer;

        /// <summary>
        /// Создаёт Mongo-контекст для отдельной тестовой базы и заполняет её начальными данными.
        /// </summary>
        public EfDatabaseFixture()
        {
            DbContext = new MongoContext(new MongoDbSettings
            {
                ConnectionString = "mongodb://127.0.0.1:27017/?serverSelectionTimeoutMS=3000",
                DatabaseName = "promocode_factory_giving_to_customer_tests"
            });

            _efTestDbInitializer= new EfTestDbInitializer(DbContext);
            _efTestDbInitializer.InitializeDb();
        }

        /// <summary>
        /// Очищает тестовую MongoDB-базу после завершения тестов.
        /// </summary>
        public void Dispose()
        {
            _efTestDbInitializer.CleanDb();
        }

        /// <summary>
        /// Mongo-контекст, который используют component-тесты и тестовые репозитории.
        /// </summary>
        public MongoContext DbContext { get; private set; }
    }
}
