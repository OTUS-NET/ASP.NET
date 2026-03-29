using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().AsQueryable();
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.ToListAsync(ct);
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(e => e.Id == id);
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(
        IEnumerable<Guid> ids,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(e => ids.Contains(e.Id));
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(
        Expression<Func<T, bool>> predicate,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(predicate);
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.ToListAsync(ct);
    }

    public async Task Add(T entity, CancellationToken ct)
    {
        await context.Set<T>().AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        var rowAffected = await context.SaveChangesAsync(ct);

        if(rowAffected == 0)
            throw new EntityNotFoundException(typeof(T), entity.Id);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var rowAffected = await context
            .Set<T>()
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(ct);

        if (rowAffected == 0)
            throw new EntityNotFoundException(typeof(T), id);
    }
}
