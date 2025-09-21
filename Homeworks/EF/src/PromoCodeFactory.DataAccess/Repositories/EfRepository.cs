using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DataContext _context;
        public EfRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public async Task<T> AddAsync(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            await _context.Set<T>().AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var entity = _context.Set<T>().FirstOrDefault(x => x.Id == id);
            if (entity is null)
                return false;

            _context.Set<T>().Remove(entity);
            var count = await _context.SaveChangesAsync();
            return count != 0;  
        }

        public async Task<T> UpdateAsync(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            var entity = _context.Set<T>().FirstOrDefault(x => x.Id == item.Id);
            if (entity is null)
                throw new KeyNotFoundException($"Entity with id= {item.Id} not found");

            _context.Set<T>().Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
