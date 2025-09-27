using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EfRepository<T>(EfContext context) : IRepository<T>
    where T : BaseEntity
{
    public DbSet<T>  Items { get; set; }
    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        //await using var context = await contextFactory.CreateDbContextAsync();
        var result = await context.Set<T>().ToListAsync();
        return result;
    }

    /// <inheritdoc />
    public async Task<T> GetByIdAsync(Guid id)
    {
        //await using var context = await contextFactory.CreateDbContextAsync();
        var result = await context.Set<T>().SingleAsync(x => x.Id == id);
        return result;
    }

    /// <inheritdoc />
    public async Task AddAsync(Customer customer)
    {
        var presentCustomer = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == customer.Id);
        if(presentCustomer != null)
            throw new Exception("Customer already exists");
        
        await context.AddAsync(customer);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<T> AddAsync(T entity)
    {
        //await using var context = await contextFactory.CreateDbContextAsync();
        var result = await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task<T> UpdateAsync(T entity)
    {
        //await using var context = await contextFactory.CreateDbContextAsync();
        var result = context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(T entity)
    {
        //await using var context = await contextFactory.CreateDbContextAsync();
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }
}