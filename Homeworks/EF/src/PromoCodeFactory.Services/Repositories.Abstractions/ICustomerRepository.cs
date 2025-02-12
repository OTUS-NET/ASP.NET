using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Contracts.Customer;

namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface ICustomerRepository
{
    Task<Customer?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Customer>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking, 
        CustomerFilterDto? customerFilterDto = null);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    void Update(Customer customer);
    void Delete(Customer customer);
    Task AddRangeIfNotExistsAsync(IEnumerable<Customer> customers, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}