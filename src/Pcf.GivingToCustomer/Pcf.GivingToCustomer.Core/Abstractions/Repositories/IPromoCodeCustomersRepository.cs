using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Repositories
{
    public interface IPromoCodeCustomersRepository
    {
        Task<List<PromoCodeCustomer>> GetAllCustomerPromoCodesByCustomerIdAsync(Guid id);
        Task<List<PromoCodeCustomer>> GetAllCustomersByPromoCodeIdAsync(Guid id);
    }
}
