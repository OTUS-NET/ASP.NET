using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Factories
{
    public class PromoCodeFactory
    {
        private IRepository<PromoCode> _promoCodeRepository;
        private IRepository<Preference> _preferenceRepository;
        private IRepository<Customer> _customerRepository;
        private IRepository<Employee> _employeeRepository;

        public PromoCodeFactory(
            IRepository<PromoCode> promoCodeRepository,
            IRepository<Preference> preferenceRepository,
            IRepository<Customer> customerRepository,
            IRepository<Employee> employeeRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
            _promoCodeRepository = promoCodeRepository;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="promoCode"></param>
        /// <returns></returns>
        public async Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            if (!await IsValidAsync(request))
            {
                return false;
            }

            var partner = await _employeeRepository.GetByIdAsync(request.PartnerId);
            var preference = await _preferenceRepository.GetByIdAsync(request.PreferenceId);

            var customers = await _customerRepository.GetAllAsync(x => 
                x.Preferences.FirstOrDefault(p => p.Id == request.PreferenceId) != null);

            foreach (var customer in customers)
            {
                var promoCode = new PromoCode()
                {
                    Code = request.PromoCode,
                    PartnerManager = partner,
                    Preference = preference,
                    ServiceInfo = request.ServiceInfo,
                    BeginDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Customer = customer,
                };

                await _promoCodeRepository.AddAsync(promoCode);
            }

            return true;
        }

        /// <summary>
        /// Валидация запроса на создание промокода
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<bool> IsValidAsync(GivePromoCodeRequest request)
        {
            var result = true;
            var employee = await _employeeRepository.GetByIdAsync(request.PartnerId);

            if (employee == null)
            {
                Console.WriteLine($"Не найден сотрудник {request.PartnerId}");
                result &= false;
            }

            var preference = await _preferenceRepository.GetByIdAsync(request.PreferenceId);

            if (preference == null)
            {
                Console.WriteLine($"Не найдено предпочтение {request.PreferenceId}");
                result &= false;
            }

            return result;
        }
    }
}
