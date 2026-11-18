using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Sevices;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromoCodeFactory.Core.Services;

public class PromoCodesService(IRepository<PromoCode> _promoCodeRepository,
    IRepository<Preference> _preferenceRepository,
    IRepository<Employee> _employeeRepository,
    IRepository<Customer> _customerRepository,
    IRepository<CustomerPromoCode> _customerPromoCodeRepository) : IPromoCodesService

{
    public async Task<IEnumerable<PromoCode>> GetAsync(bool withIncludes = false, CancellationToken ct = default)
    {
        return await _promoCodeRepository.GetAll(withIncludes, ct);
    }

    public async Task<PromoCode?> GetByIdAsync(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        return await _promoCodeRepository.GetById(id, withIncludes, ct);
    }

    public async Task<PromoCode> CreateAsync(PromoCode promoCode, CancellationToken ct = default)
    {
        var preference = await _preferenceRepository.GetById(promoCode.Preference.Id, false, ct)
            ?? throw new ArgumentException("Preference not found");

        var employee = await _employeeRepository.GetById(promoCode.PartnerManager.Id, false, ct)
            ?? throw new ArgumentException("PartnerManager not found");

        promoCode.Preference = preference;
        promoCode.PartnerManager = employee;

        var customers = await _customerRepository.GetWhere(
            c => c.Preferences.Any(p => p.Id == preference.Id),
            false, ct);

        foreach (var customer in customers)
        {
            promoCode.CustomerPromoCodes.Add(new CustomerPromoCode
            {
                CustomerId = customer.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                AppliedAt = null
            });
        }
        await _promoCodeRepository.Add(promoCode, ct);

        return promoCode;
    }

    public async Task ApplyPromoCodeAsync(Guid promoCodeId, Guid customerId, CancellationToken ct = default)
    {
        var customerPromoCode = (await _customerPromoCodeRepository.GetWhere(
            x => x.PromoCodeId == promoCodeId && x.CustomerId == customerId,
            withIncludes: false,
            ct: ct))
            .FirstOrDefault();

        if (customerPromoCode == null)
        {
            throw new ArgumentException("Промокод не найден или не был выдан этому клиенту.");
        }

        if (customerPromoCode.AppliedAt != null)
        {
            throw new InvalidOperationException("Этот промокод уже был применен клиентом.");
        }
        customerPromoCode.AppliedAt = DateTimeOffset.UtcNow;

        await _customerPromoCodeRepository.Update(customerPromoCode, ct);
    }
}
