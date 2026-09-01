namespace Pcf.GivingToCustomer.DataAccess
{
    /// <summary>
    /// Настройки подключения микросервиса GivingToCustomer к MongoDB.
    /// Значения заполняются из секции MongoDbSettings в appsettings.json или из переменных окружения docker-compose.
    /// </summary>
    public class MongoDbSettings
    {
        /// <summary>
        /// Строка подключения к серверу MongoDB.
        /// Для локального запуска обычно используется mongodb://127.0.0.1:27017,
        /// а внутри docker-compose - имя Mongo-контейнера.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Имя базы данных MongoDB, в которой хранятся клиенты, предпочтения и выданные промокоды.
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
