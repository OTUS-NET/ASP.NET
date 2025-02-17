using AutoMapper;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Services.Abstractions;
using PromoCodeFactory.Services.Contracts.Customer;
using PromoCodeFactory.Services.Contracts.Employee;
using PromoCodeFactory.Services.Contracts.Preference;
using PromoCodeFactory.Services.Contracts.PromoCode;
using PromoCodeFactory.Services.Repositories.Abstractions;

namespace PromoCodeFactory.Services.Implementations;

public class PromoCodeService : IPromoCodeService
{
    private readonly IPromoCodeUnitOfWork _promoCodeUnitOfWork;
    private readonly IMapper _mapper;

    public PromoCodeService(IPromoCodeUnitOfWork promoCodeUnitOfWork, IMapper mapper)
    {
        _promoCodeUnitOfWork = promoCodeUnitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PromoCodeShortDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return (await _promoCodeUnitOfWork.PromoCodeRepository.GetAllAsync(cancellationToken))
            .Select(p => _mapper.Map<PromoCodeShortDto>(p));
    }

    public async Task<bool> GiveToCustomersWithPreferenceAsync(
        GivePromoCodeDto givePromoCodeDto, 
        CancellationToken cancellationToken)
    {
        var preference = (await _promoCodeUnitOfWork.PreferenceRepository.GetAllAsync(
            cancellationToken,
            false,
            new PreferenceFilterDto { Names = [givePromoCodeDto.Preference] })).FirstOrDefault();
        
        if (preference is null)
            return false;
        
        var customersWithPreference = (await _promoCodeUnitOfWork.CustomerRepository.GetAllAsync(
            cancellationToken,
            false,
            new CustomerFilterDto
            {
                Preferences = new List<string>() {givePromoCodeDto.Preference},
            })).ToList();

        if (customersWithPreference.Count == 0)
            return false;

        var partnerManager = (await _promoCodeUnitOfWork.EmployeeRepository.GetAllAsync(
            cancellationToken,
            employeeFilterDto: new EmployeeFilterDto { Names = [givePromoCodeDto.PartnerName] })).FirstOrDefault();
        
        foreach (var customer in customersWithPreference)
        {
            var promoCode = _mapper.Map<PromoCode>(givePromoCodeDto);
            promoCode.CustomerId = customer.Id;
            promoCode.PreferenceId = preference.Id;
            customer.PromoCodes.Add(promoCode);
            
            if (partnerManager is not null)
                promoCode.PartnerManagerId = partnerManager.Id;
            
            await _promoCodeUnitOfWork.PromoCodeRepository.AddAsync(promoCode, cancellationToken);
        }
        
        await _promoCodeUnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}