using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System.Linq.Expressions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    protected readonly PromoCodeFactoryDbContext _context = context;

    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task Add(T entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        IQueryable<T> query = _dbSet;

        var entity = await GetById(id, false, ct);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = _dbSet;

        if (withIncludes)
            query = ApplyIncludes(query);

        return await query.ToListAsync(ct);


    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {

        IQueryable<T> query = _dbSet;

        if (withIncludes)
            query = ApplyIncludes(query);

        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = _dbSet;

        if (withIncludes)
            query = ApplyIncludes(query);

        return await query.Where(x=>ids.Contains(x.Id)).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = _dbSet;

        if (withIncludes)
            query = ApplyIncludes(query);

        return await query.Where(predicate).ToListAsync(ct);
    }

    public Task Update(T entity, CancellationToken ct)
    {
        _context.Entry(entity).State = EntityState.Modified;

        return Task.CompletedTask;
    }

}
