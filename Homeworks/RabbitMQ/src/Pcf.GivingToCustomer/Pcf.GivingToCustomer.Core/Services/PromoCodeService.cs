using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.Mappers;
using Pcf.GivingToCustomer.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services
{
    public class PromoCodeService(
         IRepository<PromoCode> promoCodesRepository,
         IRepository<Preference> preferencesRepository,
         IRepository<Customer> customersRepository) : IPromoCodeService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository = promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository = preferencesRepository;
        private readonly IRepository<Customer> _customersRepository = customersRepository;

        public async Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetByIdAsync(request.PreferenceId);

            if (preference == null)
            {
                return false;
            }

            //  Получаем клиентов с этим предпочтением:
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference, customers);

            await _promoCodesRepository.AddAsync(promoCode);
            return true;
        }
    }
}
