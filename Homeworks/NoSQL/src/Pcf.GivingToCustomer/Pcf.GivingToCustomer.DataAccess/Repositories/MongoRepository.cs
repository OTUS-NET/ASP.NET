using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    /// <summary>
    /// Универсальный MongoDB-репозиторий для доменных сущностей микросервиса GivingToCustomer.
    /// Реализует существующий контракт IRepository, чтобы контроллеры продолжали работать без изменения публичного API.
    /// </summary>
    /// <typeparam name="T">Тип доменной сущности, наследуемой от BaseEntity.</typeparam>
    public class MongoRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        private readonly MongoContext _context;
        private readonly IMongoCollection<T> _collection;

        /// <summary>
        /// Создаёт репозиторий и выбирает Mongo-коллекцию для типа T.
        /// </summary>
        /// <param name="context">Контекст MongoDB с подключением и настройками коллекций.</param>
        public MongoRepository(MongoContext context)
        {
            _context = context;
            _collection = context.GetCollection<T>();
        }

        /// <summary>
        /// Возвращает все документы коллекции.
        /// После чтения восстанавливает навигационные свойства, которые не хранятся в Mongo как EF-прокси.
        /// </summary>
        /// <returns>Список всех сущностей типа T.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await _collection.Find(_ => true).ToListAsync();

            await HydrateAsync(entities);

            return entities;
        }

        /// <summary>
        /// Ищет один документ по доменному идентификатору Id.
        /// После чтения дозаполнямт связанные объекты, необходимые контроллерам и моделям ответа.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        /// <returns>Найденная сущность или нул, если документа с таким Id нет.</returns>
        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            await HydrateAsync(entity);

            return entity;
        }

        /// <summary>
        /// Возвращает набор документов по списку идентификаторов.
        /// Используется, например, при выборе предпочтений клиента по PreferenceIds из запроса.
        /// </summary>
        /// <param name="ids">Список идентификаторов сущностей.</param>
        /// <returns>Коллекция найденных сущностей.</returns>
        public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var entities = await _collection.Find(x => ids.Contains(x.Id)).ToListAsync();

            await HydrateAsync(entities);

            return entities;
        }

        /// <summary>
        /// Возвращает первый элемент, подходящий под LINQ-предикат из существующего интерфейса репозитория.
        /// Данные сначала читаются и гидратируются, чтобы предикаты по навигационным свойствам работали как раньше в EF.
        /// </summary>
        /// <param name="predicate">Условие фильтрации.</param>
        /// <returns>Первая подходящая сущность или null.</returns>
        public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = await GetAllAsync();

            return entities.FirstOrDefault(predicate.Compile());
        }

        /// <summary>
        /// Возвращает все элементы, подходящие под LINQ-предикат из существующего интерфейса репозитория.
        /// Фильтрация выполняется в памяти после чтения из MongoDB, потому что часть текущих предикатов обращается к восстановленным связям.
        /// </summary>
        /// <param name="predicate">Условие фильтрации.</param>
        /// <returns>Список подходящих сущностей.</returns>
        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = await GetAllAsync();

            return entities.Where(predicate.Compile()).ToList();
        }

        /// <summary>
        /// Добавляет новый документ в MongoDB.
        /// Перед сохранением удаляет циклические навигационные ссылки, чтобы документ был сериализуемым.
        /// </summary>
        /// <param name="entity">Создаваемая сущность.</param>
        public async Task AddAsync(T entity)
        {
            PrepareForSave(entity);

            await _collection.InsertOneAsync(entity);
        }

        /// <summary>
        /// Полностью заменяет существующий документ по Id.
        /// Перед заменой подготавливает объект к Mongo-сериализации так же, как при добавлении.
        /// </summary>
        /// <param name="entity">Обновлённая сущность.</param>
        public async Task UpdateAsync(T entity)
        {
            PrepareForSave(entity);

            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }

        /// <summary>
        /// Удаляет документ из MongoDB по Id переданной сущности.
        /// </summary>
        /// <param name="entity">Удаляемая сущность.</param>
        public async Task DeleteAsync(T entity)
        {
            await _collection.DeleteOneAsync(x => x.Id == entity.Id);
        }

        /// <summary>
        /// Восстанавливает связанные данные для одной сущности.
        /// Метод нужен как общий вход для GetByIdAsync и делегирует работу перегрузке со списком.
        /// </summary>
        /// <param name="entity">Сущность, для которой нужно восстановить связи.</param>
        private async Task HydrateAsync(T entity)
        {
            if (entity == null)
                return;

            await HydrateAsync(new[] { entity });
        }

        /// <summary>
        /// Восстанавливает связанные данные для набора сущностей после чтения из MongoDB.
        /// Для Customer заполняются Preference и выданные PromoCode, для PromoCode заполняется Preference.
        /// </summary>
        /// <param name="entities">Сущности, прочитанные из MongoDB.</param>
        private async Task HydrateAsync(IEnumerable<T> entities)
        {
            if (typeof(T) == typeof(Customer))
            {
                var customers = entities.Cast<Customer>().ToList();
                await HydrateCustomersAsync(customers);
            }

            if (typeof(T) == typeof(PromoCode))
            {
                var promoCodes = entities.Cast<PromoCode>().ToList();
                await HydratePromoCodesAsync(promoCodes);
            }
        }

        /// <summary>
        /// Дозаполняет клиентов связанными предпочтениями и промокодами.
        /// Это заменяет EF  загрузку, который раньше автоматически подгружал Customer.Preferences и Customer.PromoCodes.
        /// </summary>
        /// <param name="customers">Список клиентов, прочитанных из MongoDB.</param>
        private async Task HydrateCustomersAsync(List<Customer> customers)
        {
            if (customers.Count == 0)
                return;

            // Собираем все PreferenceId одним списком, чтобы получить справочник предпочтений одним запросом к MongoDB.
            var preferenceIds = customers
                .SelectMany(x => x.Preferences ?? Array.Empty<CustomerPreference>())
                .Select(x => x.PreferenceId)
                .Distinct()
                .ToList();

            var preferences = await _context.GetCollection<Preference>()
                .Find(x => preferenceIds.Contains(x.Id))
                .ToListAsync();

            // Возвращаем объект Preference в каждую связь CustomerPreference, потому что модели ответа читают x.Preference.Name.
            var preferencesById = preferences.ToDictionary(x => x.Id);
            foreach (var customer in customers)
            {
                customer.Preferences ??= new List<CustomerPreference>();
                foreach (var customerPreference in customer.Preferences)
                {
                    customerPreference.Customer = customer;
                    preferencesById.TryGetValue(customerPreference.PreferenceId, out var preference);
                    customerPreference.Preference = preference;
                }
            }

            // Находим все промокоды, которые были выданы прочитанным клиентам.
            var customerIds = customers.Select(x => x.Id).ToList();
            var promoCodes = await _context.GetCollection<PromoCode>()
                .Find(x => x.Customers.Any(c => customerIds.Contains(c.CustomerId)))
                .ToListAsync();

            // Собираем Customer.PromoCodes так, чтобы CustomerResponse мог построить список выданных промокодов.
            foreach (var customer in customers)
            {
                customer.PromoCodes = promoCodes
                    .SelectMany(promoCode => (promoCode.Customers ?? Array.Empty<PromoCodeCustomer>())
                        .Where(x => x.CustomerId == customer.Id)
                        .Select(x => new PromoCodeCustomer
                        {
                            Id = x.Id,
                            CustomerId = customer.Id,
                            Customer = customer,
                            PromoCodeId = promoCode.Id,
                            PromoCode = promoCode
                        }))
                    .ToList();
            }
        }

        /// <summary>
        /// Дозаполняет промокоды связанным предпочтением.
        /// Связь хранится через PreferenceId, а объект Preference восстанавливается отдельным запросом к коллекции preferences.
        /// </summary>
        /// <param name="promoCodes">Список промокодов, прочитанных из MongoDB.</param>
        private async Task HydratePromoCodesAsync(List<PromoCode> promoCodes)
        {
            if (promoCodes.Count == 0)
                return;

            var preferenceIds = promoCodes.Select(x => x.PreferenceId).Distinct().ToList();
            var preferences = await _context.GetCollection<Preference>()
                .Find(x => preferenceIds.Contains(x.Id))
                .ToListAsync();

            var preferencesById = preferences.ToDictionary(x => x.Id);
            foreach (var promoCode in promoCodes)
            {
                preferencesById.TryGetValue(promoCode.PreferenceId, out var preference);
                promoCode.Preference = preference;
                promoCode.Customers ??= new List<PromoCodeCustomer>();
            }
        }

        /// <summary>
        /// Подготавливает доменную сущность к сохранению в MongoDB.
        /// Убирает циклические навигационные ссылки, которые были нужны в памяти, но не должны попадать в BSON-документ.
        /// </summary>
        /// <param name="entity">Сущность перед вставкой или заменой документа.</param>
        private static void PrepareForSave(T entity)
        {
            if (entity is Customer customer)
            {
                customer.Preferences ??= new List<CustomerPreference>();
                customer.PromoCodes ??= new List<PromoCodeCustomer>();

                foreach (var customerPreference in customer.Preferences)
                {
                    customerPreference.Customer = null;
                    customerPreference.Preference = null;
                }
            }

            if (entity is PromoCode promoCode)
            {
                promoCode.Preference = null;
                promoCode.Customers ??= new List<PromoCodeCustomer>();

                foreach (var promoCodeCustomer in promoCode.Customers)
                {
                    promoCodeCustomer.Customer = null;
                    promoCodeCustomer.PromoCode = null;
                    if (promoCodeCustomer.Id == Guid.Empty)
                        promoCodeCustomer.Id = Guid.NewGuid();
                }
            }
        }
    }
}
