using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain;

namespace Pcf.Administration.DataAccess.Repositories;

public class MongoRepository<T> : IRepository<T> where T: BaseEntity
{
    private readonly IMongoCollection<T> _entityCollection;

    public MongoRepository(IMongoCollection<T> entityCollection)
    {
        _entityCollection = entityCollection;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _entityCollection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetByIdAsync(ObjectId id)
    {
        return await _entityCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<ObjectId> ids)
    {
        return await _entityCollection.Find(e => ids.Contains(e.Id)).ToListAsync();
    }

    public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
    {
        return await _entityCollection.Find(predicate).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
    {
        return await _entityCollection.Find(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _entityCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        await _entityCollection.ReplaceOneAsync(e => e.Id == entity.Id, entity);
    }

    public async Task DeleteAsync(T entity)
    {
        await _entityCollection.DeleteOneAsync(e => e.Id == entity.Id);
    }
}