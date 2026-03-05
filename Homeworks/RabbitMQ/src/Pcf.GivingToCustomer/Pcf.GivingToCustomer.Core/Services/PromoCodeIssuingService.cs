using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Abstractions.Services.Models;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.Core.Services
{
    public class PromoCodeIssuingService : IPromoCodeIssuingService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;

        public PromoCodeIssuingService(
            IRepository<PromoCode> promoCodesRepository,
            IRepository<Preference> preferencesRepository,
            IRepository<Customer> customersRepository)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
        }

        public async Task<bool> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeToCustomersWithPreferenceCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.PreferenceId == Guid.Empty) throw new ArgumentException("PreferenceId is required", nameof(command));
            if (command.PartnerId == Guid.Empty) throw new ArgumentException("PartnerId is required", nameof(command));
            if (string.IsNullOrWhiteSpace(command.PromoCode)) throw new ArgumentException("PromoCode is required", nameof(command));

            var promoCodeId = command.PromoCodeId == Guid.Empty ? Guid.NewGuid() : command.PromoCodeId;

            var existing = await _promoCodesRepository.GetByIdAsync(promoCodeId);
            if (existing != null)
                return true;

            var preference = await _preferencesRepository.GetByIdAsync(command.PreferenceId);
            if (preference == null)
                return false;

            var customers = (await _customersRepository.GetWhere(d =>
                d.Preferences.Any(x => x.Preference.Id == preference.Id))).ToList();

            var promoCode = new PromoCode
            {
                Id = promoCodeId,
                PartnerId = command.PartnerId,
                Code = command.PromoCode,
                ServiceInfo = command.ServiceInfo,
                BeginDate = command.BeginDate,
                EndDate = command.EndDate,
                Preference = preference,
                PreferenceId = preference.Id,
                Customers = new List<PromoCodeCustomer>()
            };

            foreach (var customer in customers)
            {
                promoCode.Customers.Add(new PromoCodeCustomer
                {
                    CustomerId = customer.Id,
                    Customer = customer,
                    PromoCodeId = promoCode.Id,
                    PromoCode = promoCode
                });
            }

            await _promoCodesRepository.AddAsync(promoCode);

            return true;
        }
    }
}

