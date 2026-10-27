using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Sevices;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Services;

public class CustomerService(IRepository<Customer> _customerRepository, IRepository<Preference> _preferenceRepository) : ICustomerService
{
    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(bool withIncludes = false, CancellationToken ct = default)
    {
        return await _customerRepository.GetAll(withIncludes, ct);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        return await _customerRepository.GetById(id, withIncludes, ct);
    }

    public async Task CreateAsync(Customer customer, Guid[] guids, CancellationToken ct = default)
    {
        var preferences = await _preferenceRepository.GetByRangeId(guids, false, ct);
        customer.Preferences = preferences.ToList();

        await _customerRepository.Add(customer, ct);
    }

    public async Task<Customer?> UpdateAsync(Guid id, Customer request, Guid[] guids, CancellationToken ct = default)
    {
        var customer = await _customerRepository.GetById(id, withIncludes: true, ct);
        if (customer == null)
        {
            return null;
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;

        var newPreferences = await _preferenceRepository.GetByRangeId(guids, false, ct);

        customer.Preferences.Clear();
        foreach (var preference in newPreferences)
        {
            customer.Preferences.Add(preference);
        }

        await _customerRepository.Update(customer, ct);

        return customer;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _customerRepository.Delete(id, ct);
    }

}
