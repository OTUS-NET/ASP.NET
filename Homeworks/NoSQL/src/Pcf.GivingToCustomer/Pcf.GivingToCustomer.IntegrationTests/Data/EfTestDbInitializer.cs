using Pcf.GivingToCustomer.DataAccess.Data;

namespace Pcf.GivingToCustomer.IntegrationTests.Data
{
    /// <summary>
    /// Инициализатор тестовой MongoDB-базы.
    /// Имя класса сохранено прежним после миграции с EF, чтобы минимально менять существующую тестовую инфраструктуру.
    /// </summary>
    public class EfTestDbInitializer
        : IDbInitializer
    {
        private readonly DataAccess.MongoContext _context;

        /// <summary>
        /// Создаёт инициализатор для указанного тестового Mongo-контекста.
        /// </summary>
        /// <param name="context">Mongo-контекст тестовой базы.</param>
        public EfTestDbInitializer(DataAccess.MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Полностью очищает тестовую базу и заполняет её тестовыми предпочтениями и клиентами.
        /// </summary>
        public void InitializeDb()
        {
            CleanDb();

            _context.GetCollection<Core.Domain.Preference>().InsertMany(TestDataFactory.Preferences);
            _context.GetCollection<Core.Domain.Customer>().InsertMany(TestDataFactory.Customers);
        }

        /// <summary>
        /// Удаляет тестовую MongoDB-базу целиком.
        /// Используется перед заполнением и при освобождении fixture.
        /// </summary>
        public void CleanDb()
        {
            _context.Client.DropDatabase(_context.DatabaseName);
        }
    }
}
