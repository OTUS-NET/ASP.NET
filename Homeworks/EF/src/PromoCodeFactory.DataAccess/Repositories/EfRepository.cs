using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DataContext _dbContext;
    protected readonly DbSet<T> _entitySet;

    public EfRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
        _entitySet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbContext.AddAsync(entity);

        SaveChanges();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync() => (await _dbContext.Set<T>().ToListAsync()).AsEnumerable();

    public virtual async Task<T> GetByIdAsync(Guid id) => await _dbContext.Set<T>().FindAsync(id);

    /// <summary>
    /// Сохранить изменения.
    /// </summary>
    public virtual void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _entitySet.Update(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _entitySet.Remove(entity);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}