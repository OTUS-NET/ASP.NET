using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Data;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T>
        where T : BaseEntity
    {

        private readonly PromoCodeFactoryContext _context;
        private readonly DbSet<T> _dbSet;

        public EfRepository(PromoCodeFactoryContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await BuildQuery().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await BuildQuery().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        private IQueryable<T> BuildQuery()
        {
            if (typeof(T) == typeof(Employee))
            {
                return (IQueryable<T>)_context.Employees
                    .Include(x => x.Role);
            }

            if (typeof(T) == typeof(Customer))
            {
                return (IQueryable<T>)_context.Customers
                    .Include(x => x.CustomerPreferences)
                        .ThenInclude(x => x.Preference)
                    .Include(x => x.PromoCodes);
            }

            if (typeof(T) == typeof(PromoCode))
            {
                return (IQueryable<T>)_context.PromoCodes
                    .Include(x => x.Preference)
                    .Include(x => x.PartnerManager)
                    .Include(x => x.Customer);
            }

            return _dbSet.AsQueryable();
        }
    }
}