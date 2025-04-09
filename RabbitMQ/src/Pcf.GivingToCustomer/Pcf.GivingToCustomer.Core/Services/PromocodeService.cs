
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.Shared.Messaging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services;

public class PromocodeService(
    IRepository<PromoCode> promoCodesRepository,
    IRepository<Preference> preferencesRepository,
    IRepository<Customer> customersRepository)
{
    private readonly IRepository<PromoCode> _promoCodesRepository = promoCodesRepository;
    private readonly IRepository<Preference> _preferencesRepository = preferencesRepository;
    private readonly IRepository<Customer> _customersRepository = customersRepository;

    public async Task GivePromoCodesToCustomersWithPreference(PromoCodeAppliedEvent dto)
    {
        //Получаем предпочтение по имени
        var preference = await _preferencesRepository.GetByIdAsync(dto.PreferenceId)
                        ?? throw new ArgumentNullException("Can't find a preference with id " + dto.PreferenceId);

        //  Получаем клиентов с этим предпочтением:
        var customers = await _customersRepository.GetWhere(d => d.Preferences.Any
            (x => x.Preference.Id == preference.Id));

        var promocode = new PromoCode
        {
            Code = dto.Code,
            BeginDate = dto.BeginDate,
            EndDate = dto.EndDate,
            PreferenceId = dto.PreferenceId,
            ServiceInfo = dto.ServiceInfo,
            Customers = customers.Select(x => new PromoCodeCustomer { CustomerId = x.Id }).ToList()
        };

        await _promoCodesRepository.AddAsync(promocode);
    }
}
