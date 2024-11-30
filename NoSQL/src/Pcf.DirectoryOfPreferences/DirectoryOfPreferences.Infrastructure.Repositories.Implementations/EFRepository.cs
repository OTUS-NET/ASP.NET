using DirectoryOfPreferences.Domain.Abstractions;
using DirectoryOfPreferences.Domain.Entity.Base;
using DirectoryOfPreferences.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace DirectoryOfPreferences.Infrastructure.Repositories.Implementations
{
    public class EFRepository<TEntity, TId>(DataContext context) : IRepository<TEntity, TId>
        where TEntity : Entity<TId> where TId : struct
    {
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default)
        {
            IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();
            return await query.ToListAsync();
        }
        public async Task<TEntity> GetByIdAsync(TId id, CancellationToken token = default)
        {
            return await context.Set<TEntity>().FindAsync(id).AsTask();
        }
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default)
        {
            context.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(TEntity entity, CancellationToken token = default)
        {
            context.Update(entity);
            await context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(TId id, CancellationToken token = default)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;
            context.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }
        public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
            => DeleteAsync(entity.Id, cancellationToken);
    }
}
