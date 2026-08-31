namespace Pcf.GivingToCustomer.DataAccess.Data
{
    /// <summary>
    /// Инициализатор MongoDB для микросервиса GivingToCustomer.
    /// Создаёт начальное состояние базы из FakeDataFactory при старте приложения.
    /// </summary>
    public class MongoDbInitializer
        : IDbInitializer
    {
        private readonly MongoContext _context;

        /// <summary>
        /// Создаёт инициализатор для указанного Mongo-контекста.
        /// </summary>
        /// <param name="context">Контекст MongoDB, через который выполняется очистка и заполнение базы.</param>
        public MongoDbInitializer(MongoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Пересоздаёт базу данных и заполняет её начальными предпочтениями и клиентами.
        /// Логика соответствует прежнему EF-инициализатору: при старте данные приводятся к предсказуемому состоянию.
        /// </summary>
        public void InitializeDb()
        {
            _context.Client.DropDatabase(_context.DatabaseName);

            _context.GetCollection<Core.Domain.Preference>().InsertMany(FakeDataFactory.Preferences);
            _context.GetCollection<Core.Domain.Customer>().InsertMany(FakeDataFactory.Customers);
        }
    }
}
