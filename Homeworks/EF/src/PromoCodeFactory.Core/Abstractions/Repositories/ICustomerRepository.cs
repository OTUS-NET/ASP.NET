using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface ICustomerRepository : IRepository<Customer> 
    {
        Task<Customer> GetCustomerWithAllProperties(Guid id);
        Task<IEnumerable<Customer>> GetCustomersWithAllProperties();
        Task<Customer> UpdateCustomerAsync(Customer item);
    }
}
