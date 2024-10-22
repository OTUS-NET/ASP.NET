using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core;

namespace PromoCodeFactory.DataAccess.Repositories.Impl;

public class EfRepository<T>(PromoCodesDbContext dbContext) : IRepository<T>
    where T : BaseEntity
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        
        if (entity != null)
        {
            _dbContext.Set<T>().Remove(entity);
            
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task LoadRelatedDataAsync<TProperty>(T entity, Expression<Func<T, TProperty>> navigationProperty)
    {
        var propertyName = ((MemberExpression)navigationProperty.Body).Member.Name;
        
        var entry = _dbContext.Entry(entity);
        
        if (typeof(TProperty).IsGenericType && typeof(TProperty).GetGenericTypeDefinition() == typeof(ICollection<>))
        {
            await entry.Collection(propertyName).LoadAsync();
        }
        else
        {
            await entry.Reference(propertyName).LoadAsync();
        }
    }
}