namespace DirectoryOfPreferences.Infrastructure.Repositories.Implementations.EntityFramework
{
    public class EFRepository<TEntity, TId>(ApplicationDbContext context) : IRepository<TEntity, TId>
        where TEntity : Entity<TId>
        where TId : struct
    {
        private static readonly char[] IncludeSeparator = [','];
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                                    string? includes = null,
                                                                    bool asNoTracking = false,
                                                                    CancellationToken token = default)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();

            if (filter is not null)
                query = query.Where(filter);

            if (includes is not null && includes.Any())
                includes.Split(IncludeSeparator, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(includeEntity => query = query.Include(includeEntity));

            if (asNoTracking)
                query = query.AsNoTracking();
            return await query.ToListAsync(token);
        }
        public virtual async Task<TEntity?> GetByIdAsync(Expression<Func<TEntity, bool>> filter,
                                                  string? includes = null,
                                                  bool asNoTracking = false,
                                                  CancellationToken token = default)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();

            if (filter is not null)
                query = query.Where(filter);

            if (includes is not null && includes.Any())
                includes.Split(IncludeSeparator, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(includeEntity => query = query.Include(includeEntity));

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.SingleOrDefaultAsync(token);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default)
        {
            context.Add(entity);
            await context.SaveChangesAsync(token);
            return entity;
        }
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken token = default)
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync(token);
        }
        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken token = default)
        {
            if (entity is null)
                return false;
            context.Entry(entity).State = EntityState.Deleted;
            await context.SaveChangesAsync(token);
            return true;
        }
    }
}
