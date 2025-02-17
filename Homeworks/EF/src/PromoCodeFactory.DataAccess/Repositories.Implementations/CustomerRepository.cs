using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Customer;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.DataAccess.Repositories.Implementations;

public class CustomerRepository : EfRepository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(DatabaseContext databaseContext) : base(databaseContext) { }

    public async Task<IEnumerable<Customer>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking,
        CustomerFilterDto customerFilterDto = null)
    {
        var query = GetAll(asNoTracking);

        if (customerFilterDto != null)
        {
            if (customerFilterDto.Preferences is { Count: > 0 })
            {
                query = query.Where(c => c.Preferences.Any(
                    p => customerFilterDto.Preferences.Contains(p.Name)));
            }
            
            if (customerFilterDto.PromoCodes is { Count: > 0 })
            {
                query = query.Where(c => c.PromoCodes.Any(
                    p => customerFilterDto.PromoCodes.Contains(p.Code)));
            }
        }
        
        return await query.ToListAsync(cancellationToken);
    }
}