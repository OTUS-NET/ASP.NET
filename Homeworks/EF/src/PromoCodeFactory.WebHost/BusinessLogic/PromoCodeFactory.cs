using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.BusinessLogic
{
    public class PromoCodeFactoryBl
    {
        private IRepository<PromoCode> _promoCodeRepository;
        private IRepository<Preference> _preferenceRepository;
        private IRepository<Customer> _customerRepository;
        private IRepository<Employee> _employeeRepository;
        public PromoCodeFactoryBl(
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
            if (!await IsRequestValidAsync(request))
                return false;

            //Все клиенты, удовлетворяющие условию по предпочтению
            var customers = await _customerRepository.GetAllAsync(
                x => x.Preferences.FirstOrDefault(p => p.Id == request.PreferenceId) != null
                );

            //Для каждого клиента добавляем промокод
            foreach (var customer in customers)
            {
                var promoCode = new PromoCode()
                {
                    Code = request.PromoCode,
                    PartnerManagerId = request.PartnerId,
                    PreferenceId = request.PreferenceId,
                    ServiceInfo = request.ServiceInfo,
                    BeginDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    CustomerId = customer.Id,
                };

                //Лучше конечно делать CreateRange скопом вне цикла
                var created = await _promoCodeRepository.CreateAsync(promoCode);
            }

            return true;
        }

        /// <summary>
        /// Валидация запроса на создание промокода
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<bool> IsRequestValidAsync(GivePromoCodeRequest request)
        {
            bool result = true;

            //Валидируем сотрудника, только если его Id указан
            var partnerId = request.PartnerId;
            if (partnerId != Guid.Empty)
            {
                var employeeFound = await _employeeRepository.GetByIdAsync(partnerId);
                if (employeeFound == null)
                {
                    Console.WriteLine($"Ошибка при создании промокода: не найден сотрудник партнера c Id {partnerId}");
                    result &= false;
                }
            }

            //Преференс валидируем всегда
            var preferenceId = request.PreferenceId;
            var prefFound = await _preferenceRepository.GetByIdAsync(preferenceId);
            if (prefFound == null)
            {
                Console.WriteLine($"Ошибка при создании промокода: не найдено предпочтение  c Id {preferenceId}");
                result &= false;
            }

            return result;
        }
    }
}
