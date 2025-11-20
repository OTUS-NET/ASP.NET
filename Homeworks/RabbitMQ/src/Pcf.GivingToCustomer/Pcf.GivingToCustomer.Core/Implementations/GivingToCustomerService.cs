using System.Linq;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Abstractions;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Implementations;

public class GivingToCustomerService(IRepository<PromoCode> promoCodesRepository,
        IRepository<Preference> preferencesRepository,
        IRepository<Customer> customersRepository) : IGivingToCustomerService
{
    public async Task GiveToCustomer(PromoCode promoCode)
    {
        var preference = await preferencesRepository.GetByIdAsync(promoCode.PreferenceId);

        if (preference == null)
        {
            return;
        }

        //  Получаем клиентов с этим предпочтением:
        var customers = await customersRepository
            .GetWhere(d => d.Preferences.Any(x =>
                x.Preference.Id == preference.Id));

        await promoCodesRepository.AddAsync(promoCode);
    }
}
