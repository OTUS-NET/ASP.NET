using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Base;
using PromoCodeFactory.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EFRepository<TEntity, TId>(DataContext context) : IRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : struct
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
           return (await context.Set<TEntity>().AsNoTracking().ToListAsync()).AsEnumerable();
        }
        public Task<TEntity> GetByIdAsync(TId id)
        {
           return context.Set<TEntity>().FindAsync(id).AsTask();
        }
        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            context.Add(entity);
            await context.SaveChangesAsync();
            return entity; 
        }
        public async Task UpdateAsync(TId id, TEntity entity)
        {
            context.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(TId id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;
            context.Remove(entity);
            await context.SaveChangesAsync(); 
        }
    }
}
