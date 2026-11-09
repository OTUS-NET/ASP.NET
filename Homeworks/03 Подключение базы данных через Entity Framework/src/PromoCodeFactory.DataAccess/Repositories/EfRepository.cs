using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Collections;
using System.Linq.Expressions;

namespace PromoCodeFactory.DataAccess.Repositories;

public class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    public async Task Add(T entity, CancellationToken ct)
    {
        context.Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var delete = await context.Set<T>()
            .Where(x=>x.Id == id)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = context.Set<T>();

        if (withIncludes)
        {
            query = ApplyIncludes(query);
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = context.Set<T>();
        if (withIncludes)
        {
            query = ApplyIncludes(query);
        }
        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        var idList = ids.ToList();

        IQueryable<T> query = context.Set<T>().Where(x => idList.Contains(x.Id));

        if (withIncludes)
        {
            query = ApplyIncludes(query);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = context.Set<T>().Where(predicate);
        if (withIncludes)
        {
            query = ApplyIncludes(query);
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        context.Update(entity);
        await context.SaveChangesAsync(ct);
    }
}
