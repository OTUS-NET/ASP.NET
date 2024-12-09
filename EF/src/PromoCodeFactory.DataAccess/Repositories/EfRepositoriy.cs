using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DataContext _dbContext;
        
        protected readonly DbSet<T> _entitySet;

        public EfRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            _entitySet = _dbContext.Set<T>();
        }

        public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _entitySet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _entitySet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<T> CreateAsync(T obj, CancellationToken cancellationToken = default)
        {
            var entity = (await _entitySet.AddAsync(obj, cancellationToken)).Entity;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
             _entitySet.Update(obj);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return obj;
        }

        public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false; 
            
            _entitySet.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }      
    }
}