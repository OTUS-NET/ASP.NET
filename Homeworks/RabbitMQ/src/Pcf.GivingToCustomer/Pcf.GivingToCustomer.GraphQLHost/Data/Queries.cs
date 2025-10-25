using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;

namespace Pcf.GivingToCustomer.GraphQLHost.Data
{
    public class Queries
    {
        // Получить все промокоды
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<PromoCode> GetPromoCodes(
            [Service] DataContext context)
        {
            return context.PromoCodes
                .Include(p => p.Preference)
                .Include(p => p.Customers)
                .ThenInclude(pc => pc.Customer);
        }


        // Получить всех клиентов
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Customer> GetCustomers(
            [Service] DataContext context)
        {
            return context.Customers
                .Include(c => c.Preferences)
                .ThenInclude(cp => cp.Preference)
                .AsSplitQuery();
        }

        // Получить все предпочтения
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Preference> GetPreferences(
            [Service] DataContext context)
        {
            return context.Preferences;
        }
    }
}
