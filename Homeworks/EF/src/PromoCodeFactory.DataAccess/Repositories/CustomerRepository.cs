using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

public class CustomerRepository : EfRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(DataContext dataContext) : base(dataContext) { }

    public override async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _dbContext.Set<Customer>()
           .Include(c => c.CustomerPreferences)
           .ToListAsync();
    }

    public override async Task<Customer> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<Customer>()
             .Include(c => c.PromoCodes)
             .Include(c => c.CustomerPreferences)
             .ThenInclude(p => p.Preference)
             .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<Customer> UpdateAsync(Customer customerUpdate)
    {
        var customer = await GetByIdAsync(customerUpdate.Id);

        if (customer == null)
            return null;

        _entitySet.Entry(customer).CurrentValues.SetValues(customerUpdate);

        customer.CustomerPreferences.Clear();
        foreach (var pref in customerUpdate.CustomerPreferences)
        {
            customer.CustomerPreferences.Add(new CustomerPreference
            {
                CustomerId = customer.Id,
                PreferenceId = pref.PreferenceId
            });
        }

        await _dbContext.SaveChangesAsync();

        return customer;
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await GetByIdAsync(id);

        if (customer == null)
            return false;

        _dbContext.RemoveRange(customer.PromoCodes);
        _dbContext.Remove(customer);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<Customer>> GetAllByPreference(Guid id)
    {
        var customers = await _dbContext.Customers
            .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == id))
            .ToListAsync();

        return customers;
    }


}