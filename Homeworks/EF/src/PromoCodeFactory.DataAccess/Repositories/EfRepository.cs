using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EfRepository<T>(EfContext context) : IRepository<T>
    where T : BaseEntity
{
    /// <inheritdoc cref="IRepository{T}" />
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var result = await context.Set<T>().ToListAsync();
        return result;
    }

    /// <inheritdoc cref="IRepository{T}" />
    public async Task<T?> GetByIdAsync(Guid id)
    {
        var result = await context.Set<T>().SingleOrDefaultAsync(x => x.Id == id);
        return result;
    }

    /// <inheritdoc cref="IRepository{T}" />
    public async Task<T> AddAsync(T customer)
    {
        var presentCustomer = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == customer.Id);
        if(presentCustomer != null)
            throw new Exception("Customer already exists");
        
        var result = await context.AddAsync(customer);
        await context.SaveChangesAsync();
        return result.Entity;
    }
    
    /// <inheritdoc cref="IRepository{T}" />
    public async Task<T> UpdateAsync(T entity)
    {
        var result = context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    /// <inheritdoc cref="IRepository{T}" />
    public async Task RemoveAsync(T entity)
    {
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
}