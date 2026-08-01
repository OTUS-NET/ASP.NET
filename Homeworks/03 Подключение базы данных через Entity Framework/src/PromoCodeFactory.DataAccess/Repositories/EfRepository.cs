using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    private IQueryable<T> Query(bool withIncludes) =>
        withIncludes ? ApplyIncludes(context.Set<T>()) : context.Set<T>();

    public async Task Add(T entity, CancellationToken ct)
    {
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, ct)
            ?? throw new EntityNotFoundException(typeof(T), id);

        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        return await Query(withIncludes).ToListAsync(ct);
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        return await Query(withIncludes).FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        return await Query(withIncludes).Where(e => ids.Contains(e.Id)).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        return await Query(withIncludes).Where(predicate).ToListAsync(ct);
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        if (!await context.Set<T>().AnyAsync(e => e.Id == entity.Id, ct))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(ct);
    }
}
