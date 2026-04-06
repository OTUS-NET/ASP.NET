using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    public async Task Add(T entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await context.Set<T>().AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var entity = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            throw new EntityNotFoundException<T>(id);

        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(CancellationToken ct = default, bool withIncludes = false)
    {
        ct.ThrowIfCancellationRequested();

        IQueryable<T> query = context.Set<T>().AsNoTracking();
        if (withIncludes)
            query = ApplyIncludes(query);

        var list = await query.ToListAsync(ct);
        return list;
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        IQueryable<T> query = context.Set<T>().AsNoTracking();
        if (withIncludes)
            query = ApplyIncludes(query);

        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var idList = ids as ICollection<Guid> ?? ids.ToList();

        IQueryable<T> query = context.Set<T>().AsNoTracking();
        if (withIncludes)
            query = ApplyIncludes(query);

        var list = await query.Where(x => idList.Contains(x.Id)).ToListAsync(ct);
        return list;
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        IQueryable<T> query = context.Set<T>().AsNoTracking().Where(predicate);
        if (withIncludes)
            query = ApplyIncludes(query);

        var list = await query.ToListAsync(ct);
        return list;
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var exists = await context.Set<T>().AsNoTracking().AnyAsync(x => x.Id == entity.Id, ct);
        if (!exists)
            throw new EntityNotFoundException<T>(entity.Id);

        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(ct);
    }

}
