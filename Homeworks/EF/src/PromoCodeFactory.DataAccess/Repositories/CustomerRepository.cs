using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

public class CustomerRepository : IRepository<Customer>
{
    DatabaseContext _context;
    public CustomerRepository(DatabaseContext context)
    {
        _context = context;
    }


    public async Task AddAsync(Customer entity, CancellationToken cancellationToken)
    {
        await _context.Customers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Delete(Customer entity)
    {
        _context.Customers.Remove(entity);
        _context.SaveChanges();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(id, cancellationToken);

        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<Customer> GetAll(bool asNoTracking)
    {
        return asNoTracking
            ? _context.Customers.AsNoTracking()
            : _context.Customers;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking)
    {
        return asNoTracking
            ? await _context.Customers.AsNoTracking().ToListAsync(cancellationToken)
            : await _context.Customers.ToListAsync(cancellationToken);
    }

    public async Task<Customer> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Customers.FindAsync(id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Customer entity)
    {
        _context.Customers.Update(entity);
        _context.SaveChanges();
    }

    public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(id, cancellationToken);

        if (customer == null)
            return false;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddRangeIfNotExistsAsync(IEnumerable<Customer> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            var existingEntity = await _context.Customers.FindAsync(entity.Id, cancellationToken);
            if (existingEntity == null)
            {
                await AddAsync(entity, cancellationToken);
            }
        }
    }
}
