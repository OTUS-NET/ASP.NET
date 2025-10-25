using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.GraphQLHost.Dtos;

namespace Pcf.GivingToCustomer.GraphQLHost.Data
{
    public class Mutations
    {
        // Создание промокода
        public async Task<PromoCode> CreatePromoCode(
            PromoCodeInput input,
            [Service] DataContext context)
        {
            Guid promocodeId = Guid.NewGuid();
            var promoCode = new PromoCode
            {
                Id = promocodeId,
                Code = input.Code,
                ServiceInfo = input.ServiceInfo,
                BeginDate = input.BeginDate,
                EndDate = input.EndDate,
                PreferenceId = input.PreferenceId,
                Customers = input.CustomerIds?.Select(customerId =>
                    new PromoCodeCustomer
                    {
                        PromoCodeId = promocodeId,
                        CustomerId = customerId
                    }).ToList()
            };

            await context.PromoCodes.AddAsync(promoCode);
            await context.SaveChangesAsync();
            return promoCode;
        }

        // Создание клиента
        public async Task<Customer> CreateCustomer(
            CustomerInput input,
            [Service] DataContext context)
        {
            Guid customerId = Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                Preferences = input.PreferenceIds?.Select(preferenceId =>
                    new CustomerPreference
                    {
                        CustomerId = customerId,
                        PreferenceId = preferenceId
                    }).ToList()
            };

            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();
            return customer;
        }


        // Обновление клиента
        public async Task<Customer> UpdateCustomer(
            CustomerUpdateInput input,
            [Service] DataContext context)
        {
            var customer = await context.Customers
                .Include(c => c.Preferences)
                .FirstOrDefaultAsync(c => c.Id == input.Id) ?? throw new ArgumentException("Customer not found");
            customer.FirstName = input.FirstName;
            customer.LastName = input.LastName;
            customer.Email = input.Email;

            // Обновление предпочтений
            if (input.PreferenceIds != null)
            {
                var existingPreferenceIds = customer.Preferences
                    .Select(cp => cp.PreferenceId)
                    .ToHashSet();

                var preferencesToRemove = customer.Preferences.Where(p => !input.PreferenceIds.Contains(p.PreferenceId));

                foreach (var preferenceToRemove in preferencesToRemove)
                {
                    customer.Preferences.Remove(preferenceToRemove);
                }

                // Добавление новых предпочтений

                var newPreferenceIds = input.PreferenceIds.Except(existingPreferenceIds);

                foreach (var newPreferenceId in newPreferenceIds)
                {
                    customer.Preferences.Add(new CustomerPreference
                    {
                        CustomerId = input.Id,
                        PreferenceId = newPreferenceId
                    });
                }
            }

            await context.SaveChangesAsync();
            return customer;
        }


        // Удаление клиента
        public bool DeleteCustomer(
            Guid id,
            [Service] DataContext context)
        {
            var customer = context.Customers
                .Include(c => c.Preferences)
                .FirstOrDefault(c => c.Id == id);

            if (customer == null) return false;
            context.Customers.Remove(customer);
            context.SaveChanges();
            return true;
        }
    }
}
