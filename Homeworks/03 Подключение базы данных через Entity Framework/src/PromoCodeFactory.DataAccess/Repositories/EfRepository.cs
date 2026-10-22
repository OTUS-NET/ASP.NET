using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;
using System.Linq.Expressions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    public async Task Add(T entity, CancellationToken ct)
    {
        entity.Id = Guid.NewGuid();
        await context.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync(ct);
        if (entity == null)
            throw new EntityNotFoundException(typeof(T), id);
        context.Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(_ => true);
        if (withIncludes)
        {
            query = ApplyIncludes(query);
        }
        return await query.ToListAsync(ct);
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        return await context.Set<T>().FindAsync(id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        return await context.Set<T>().Where(_ => ids.Contains(_.Id)).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(predicate);
        if (withIncludes)
        {
            query = ApplyIncludes(query);
        }
        return await query.ToListAsync(ct);
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        if(!await context.Set<T>().Where(x => x.Id == entity.Id).AnyAsync(ct))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        context.Update(entity);
        await context.SaveChangesAsync(ct);
    }
}
