using System;
using System.Collections.Generic;
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
        /// Если сущность может быть создана вне контекста данных _dbSet.Attach(entity);
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
                .Include(customer => customer.CustomerPreferences)
                .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
        }

    }
}