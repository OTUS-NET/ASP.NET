using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfCoreRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DatabaseContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public EfCoreRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token)
        {
            return await _dbSet.ToListAsync(token);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken token)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public virtual async Task CreateAsync(T entity, CancellationToken token)
        {
            await _dbSet.AddAsync(entity, token);
            await _dbContext.SaveChangesAsync(token);
            entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == entity.Id, token);
        }

        public virtual async Task UpdateAsync(Guid id, T entity, CancellationToken token)
        {
            //var storedEntity = await TryFindEntityOrThrow(id, token);
            _dbSet.Update(entity);
            //_dbSet.Entry(storedEntity).CurrentValues.SetValues(entity);
            //_dbSet.Entry(storedEntity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(token);
        }

        public virtual async Task DeleteByIdAsync(Guid id, CancellationToken token)
        {
            var storedEntity = await TryFindEntityOrThrow(id, token);
            _dbSet.Entry(storedEntity).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync(token);
        }

        #region Utils

        /// <summary>
        /// Tries to find an Entity with the provided Key. Otherwise throws the ArgumentException.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        private async Task<T> TryFindEntityOrThrow(Guid id, CancellationToken token)
        {
            if (await _dbSet.Where(e => e.Id == id).FirstOrDefaultAsync(token) is not T storedEntity)
            {
                throw new ArgumentException($"Entity {typeof(T)} not found", nameof(id));
            }
            return storedEntity;
        }

        #endregion
    }
}
