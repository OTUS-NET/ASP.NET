using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions;
using PromoCodeFactory.Core.Abstractions.Repositories;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class EfRepository<T, TPrimaryKey> : IRepository<T, TPrimaryKey> where T : class, IEntity<TPrimaryKey>
{
    private readonly DatabaseContext _databaseContext;
    private readonly DbSet<T> _entitySet;
    
    public EfRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        _entitySet = _databaseContext.Set<T>();
    }

    public async Task AddRangeIfNotExistsAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            var existingEntity = await _entitySet.FindAsync(entity.Id, cancellationToken);
            if (existingEntity == null)
            {
                await AddAsync(entity, cancellationToken);
            }
        }
    }
    
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _entitySet.AddAsync(entity, cancellationToken);
    }

    public async Task<T> GetAsync(TPrimaryKey id, CancellationToken cancellationToken)
    {
        return await _entitySet.FindAsync(id, cancellationToken);
    }

    public virtual IQueryable<T> GetAll(bool asNoTracking = false)
    {
        return asNoTracking ? _entitySet.AsNoTracking() : _entitySet;
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
    {
        return await GetAll(asNoTracking).ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(TPrimaryKey id, CancellationToken cancellationToken)
    {
        var entity = await GetAsync(id, cancellationToken);
        if (entity == null)
        {
            return false;
        }
        Update(entity);
        return true;
    }

    public void Update(T entity)
    {
        _databaseContext.Entry(entity).State = EntityState.Modified;
    }

    public async Task<bool> DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken)
    {
        var entity = await GetAsync(id, cancellationToken);
        if (entity == null)
        {
            return false;
        }
        _entitySet.Remove(entity);
        return true;
    }

    public void Delete(T entity)
    {
        _databaseContext.Entry(entity).State = EntityState.Deleted;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _databaseContext.SaveChangesAsync(cancellationToken);
    }
}