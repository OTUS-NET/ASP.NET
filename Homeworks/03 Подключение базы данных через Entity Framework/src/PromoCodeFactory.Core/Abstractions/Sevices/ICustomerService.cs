using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Abstractions.Sevices;

public interface ICustomerService
{
    Task<IReadOnlyCollection<Customer>> GetAllAsync(bool withIncludes = false, CancellationToken ct = default);
    Task<Customer?> GetByIdAsync(Guid id, bool withIncludes = false, CancellationToken ct = default);
    Task CreateAsync(Customer customer, Guid[] guids, CancellationToken ct = default);
    Task<Customer?> UpdateAsync(Guid id, Customer request, Guid[] guids, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
