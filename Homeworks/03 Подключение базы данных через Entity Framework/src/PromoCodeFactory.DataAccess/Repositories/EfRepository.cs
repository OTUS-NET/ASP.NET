using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System.Linq.Expressions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query)
    {
        return query;
    }

    public async Task Add(T entity, CancellationToken ct)
    {
        try
        {
            await context.Set<T>().AddAsync(entity, ct);
            await context.SaveChangesAsync(ct);
        }
        catch
        {
            throw;
        }
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        try
        {
            T? item = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (item is null)
            {
                throw new InvalidOperationException($"Not found by id {id}");
            }

            context.Set<T>().Remove(item);
            await context.SaveChangesAsync(ct);
        }
        catch
        {
            throw;
        }
    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        try
        {
            IQueryable<T> query = context.Set<T>().AsNoTracking();
            if (withIncludes)
            {
                query = ApplyIncludes(query);
            }
            return await query.ToListAsync(ct);
        }
        catch
        {
            throw;
        }
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        try
        {
            IQueryable<T> query = context.Set<T>();
            if (withIncludes)
            {
                query = ApplyIncludes(query);
            }
            T? item = await query.FirstOrDefaultAsync(x => x.Id == id, ct);

            return item;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        try
        {
            IQueryable<T> query = context.Set<T>();
            if (withIncludes)
            {
                query = ApplyIncludes(query);
            }

            List<T> items = await query.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
            return items;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        try
        {
            IQueryable<T> query = context.Set<T>();
            if (withIncludes)
            {
                query = ApplyIncludes(query);
            }

            List<T> items = await query.Where(predicate).ToListAsync(ct);
            return items;
        }
        catch
        {
            throw;
        }
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        try
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync(ct);
        }
        catch
        {
            throw;
        }
    }

}
