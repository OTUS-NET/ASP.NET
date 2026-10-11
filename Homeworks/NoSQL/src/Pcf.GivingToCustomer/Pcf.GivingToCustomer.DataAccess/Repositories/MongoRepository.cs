using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class MongoRepository<T> : IRepository<T>
        where T : BaseEntity
    {
        protected readonly IMongoCollection<T> Collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            Collection = database.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await Collection.AsQueryable().ToListAsync();
            return entities.Select(AfterLoad);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await Collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
            return AfterLoad(entity);
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var entities = await Collection.AsQueryable().Where(x => ids.Contains(x.Id)).ToListAsync();
            return entities.Select(AfterLoad);
        }

        public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        {
            var entity = await Collection.AsQueryable().Where(predicate).FirstOrDefaultAsync();
            return AfterLoad(entity);
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = await Collection.AsQueryable().Where(predicate).ToListAsync();
            return entities.Select(AfterLoad);
        }

        public virtual async Task AddAsync(T entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await Collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await Collection.DeleteOneAsync(filter);
        }

        // Derived classes can override to fix up loaded documents (e.g. ensure non-null collections)
        protected virtual T AfterLoad(T entity) => entity;
    }
}
