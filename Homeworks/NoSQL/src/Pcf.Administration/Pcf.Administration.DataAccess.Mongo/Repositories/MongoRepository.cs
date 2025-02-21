using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain;
using System.Linq.Expressions;

namespace Pcf.Administration.DataAccess.Mongo.Repositories;
public class MongoRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoDbContext dbContext) : this(dbContext, null)
    {
        
    }

    public MongoRepository(MongoDbContext dbContext, string? collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            var type = typeof(T);
            collectionName = type.Name;
        }
        _collection = dbContext.Database.GetCollection<T>(collectionName);
    }

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id));
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq(e => e.Id, id)).FirstOrDefaultAsync();
    }

    public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(Builders<T>.Filter.Where(predicate)).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
    {
        return await _collection.Find(Builders<T>.Filter.In(e => e.Id, ids)).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(Builders<T>.Filter.Where(predicate)).ToListAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity);
    }
}
