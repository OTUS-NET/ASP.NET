using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Infrastructure.EntityFramework;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(id, cancellationToken);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Использовать, если уверен, что сущность уже отслеживается контекстом данных.
        /// Если сущность может быть создана вне контекста данных _dbSet. Attach(entity);
        /// </summary>
        /// <param name="entity"></param>
        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Customer> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (typeof(T) != typeof(Customer))
            {
                throw new InvalidOperationException("This method is only available for Customer entity.");
            }

            var customerDbSet = _dbSet as DbSet<Customer>;
            return await customerDbSet
                .Where(c => c.Id == id)
                .Include(c => c.CustomerPreferences)
                .ThenInclude(cp => cp.Preference)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CreateCustomersAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (typeof(T) != typeof(Customer))
            {
                throw new InvalidOperationException("This method is only available for Customer entity.");
            }

            var customer = entity as Customer;
            if (customer.CustomerPreferences != null)
            {

                foreach (var customerPreference in customer.CustomerPreferences)
                {

                    _context.CustomerPreferences.Add(customerPreference);
                }
            }

            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await GetCustomerByIdAsync(id, cancellationToken);

            // Удаляем связанные CustomerPreference записи
            _context.CustomerPreferences.RemoveRange(customer.CustomerPreferences);
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Preference> GetPreferenceByNameAsync(string name,
            CancellationToken cancellationToken = default)
        {
            if (typeof(T) != typeof(Preference))
            {
                throw new InvalidOperationException("This method is only available for Customer entity.");
            }

            return await _context.Preferences
                .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            // Получаем текущую сущность Customer из базы данных
            var existingCustomer = await _context.Customers
                .Include(c => c.CustomerPreferences)
                .FirstOrDefaultAsync(c => c.Id == customer.Id, cancellationToken);

            if (existingCustomer == null)
            {
                throw new InvalidOperationException("Customer with the specified Id does not exist.");
            }

            // Обновляем основные поля Customer
            _context.Entry(existingCustomer).CurrentValues.SetValues(customer);

            // Обновляем связанные CustomerPreference записи
            var existingPreferences = existingCustomer.CustomerPreferences.ToList();
            var newPreferences = customer.CustomerPreferences.ToList();

            // Удаляем CustomerPreference, которых больше нет в новом наборе
            foreach (var existingPreference in existingPreferences)
            {
                if (!newPreferences.Any(p =>
                        p.CustomerId == existingPreference.CustomerId &&
                        p.PreferenceId == existingPreference.PreferenceId))
                {
                    _context.CustomerPreferences.Remove(existingPreference);
                }
            }

            // Добавляем или обновляем CustomerPreference
            foreach (var preference in newPreferences)
            {
                var existingPreference = existingPreferences
                    .FirstOrDefault(p =>
                        p.CustomerId == preference.CustomerId && p.PreferenceId == preference.PreferenceId);

                if (existingPreference != null)
                {
                    // Обновляем существующую связанную сущность
                    _context.Entry(existingPreference).CurrentValues.SetValues(preference);
                }
                else
                {
                    // Добавляем новую связанную сущность
                    existingCustomer.CustomerPreferences.Add(preference);
                }
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}