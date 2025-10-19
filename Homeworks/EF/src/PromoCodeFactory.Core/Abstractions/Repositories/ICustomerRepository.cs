using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Abstractions.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<List<Customer>> GetAllByPreference(Guid id);
}
