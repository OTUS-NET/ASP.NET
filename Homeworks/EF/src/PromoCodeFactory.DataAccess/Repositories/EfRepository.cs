using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DataContext _dbContext;
        public EfRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<T> QueryWithIncludes()
        {
            IQueryable<T> query = _dbContext.Set<T>();

            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            if (entityType == null)
                return query;

            foreach (var navigation in entityType.GetNavigations())
            {
                query = query.Include(navigation.Name);
            }

            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await QueryWithIncludes().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await QueryWithIncludes().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Guid> AddAsync(T entity)
        {
            var entry = await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var entry = _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
