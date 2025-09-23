using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EfRepository<T>(IDbContextFactory<DbContext> contextFactory) : IRepository<T>
    where T : BaseEntity
{
    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var result = await context.Set<T>().ToListAsync();
        return result;
    }

    /// <inheritdoc />
    public async Task<T> GetByIdAsync(Guid id)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var result = await context.Set<T>().SingleAsync(x => x.Id == id);
        return result;
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var result = await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task<T> UpdateAsync(T entity)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        var result = context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(T entity)
    {
        await using var context = await contextFactory.CreateDbContextAsync();
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
}