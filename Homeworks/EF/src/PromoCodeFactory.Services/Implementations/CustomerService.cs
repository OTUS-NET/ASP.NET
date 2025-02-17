using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.Services.Contracts.Customer;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerUnitOfWork _customerUnitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerUnitOfWork customerUnitOfWork, IMapper mapper)
    {
        _customerUnitOfWork = customerUnitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerShortDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var customers = await _customerUnitOfWork.CustomerRepository.GetAllAsync(cancellationToken, true);
        return customers.Select(c => _mapper.Map<CustomerShortDto>(c));
    }

    public async Task<CustomerDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerUnitOfWork.CustomerRepository.GetAsync(id, cancellationToken);
        return customer == null ? null : _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto?> CreateAsync(
        CreateOrEditCustomerDto createOrEditCustomerDto, 
        CancellationToken cancellationToken)
    {
        var customer = _mapper.Map<Customer>(createOrEditCustomerDto);
        foreach (var preferenceId in createOrEditCustomerDto.PreferenceIds)
        {
            var preference = await _customerUnitOfWork.PreferenceRepository.GetAsync(preferenceId, cancellationToken);
            if (preference == null)
                return null;
            
            customer.Preferences.Add(preference);
        }

        await _customerUnitOfWork.CustomerRepository.AddAsync(customer, cancellationToken);
        await _customerUnitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<bool> EditAsync(
        Guid id,
        CreateOrEditCustomerDto createOrEditCustomerDto, 
        CancellationToken cancellationToken)
    {
        var customer = await _customerUnitOfWork.CustomerRepository.GetAsync(id, cancellationToken);
        if (customer == null)
            return false;
        
        _mapper.Map(createOrEditCustomerDto, customer);
        
        foreach (var preferenceId in createOrEditCustomerDto.PreferenceIds)
        {
            var preference = await _customerUnitOfWork.PreferenceRepository.GetAsync(preferenceId, cancellationToken);
            if (preference == null)
                return false;
            
            customer.Preferences.Add(preference);
        }
        
        _customerUnitOfWork.CustomerRepository.Update(customer);
        await _customerUnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerUnitOfWork.CustomerRepository.GetAsync(id, cancellationToken);
        if (customer == null)
            return false;
        
        _customerUnitOfWork.CustomerRepository.Delete(customer);
        await _customerUnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}