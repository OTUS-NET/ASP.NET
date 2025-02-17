using PromoCodeFactory.Services.Contracts.Customer;

namespace PromoCodeFactory.Services.Abstractions;

public interface ICustomerService
{
    Task<IEnumerable<CustomerShortDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<CustomerDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<CustomerDto?> CreateAsync(CreateOrEditCustomerDto createOrEditCustomerDto, CancellationToken cancellationToken);
    Task<bool> EditAsync(Guid id, CreateOrEditCustomerDto createOrEditCustomerDto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}