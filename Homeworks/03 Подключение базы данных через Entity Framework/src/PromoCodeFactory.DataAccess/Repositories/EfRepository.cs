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
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.Set<T>().Where(e => e.Id == id).FirstOrDefaultAsync(ct);

        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(T), id);
        }
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = context.Set<T>();

        if (withIncludes)
        {
            query = this.ApplyIncludes(query);
        }
        return await query.ToArrayAsync(ct);
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        IQueryable<T> query = context.Set<T>();

        if (withIncludes)
        {
            query = this.ApplyIncludes(query);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(e => ids.Contains(e.Id));

        if (withIncludes)
        {
            query = this.ApplyIncludes(query);
        }
        return await query.ToArrayAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(predicate);

        if (withIncludes)
        {
            query = this.ApplyIncludes(query);
        }
        return await query.ToArrayAsync(ct);
    }

    public virtual async Task Update(T entity, CancellationToken ct)
    {
        var entityToUpdate = await context.Set<T>().FirstOrDefaultAsync(e => e.Id == entity.Id);

        if (entityToUpdate is null)
        {
            throw new EntityNotFoundException(typeof(T), entity.Id);
        }
        context.Set<T>().Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(ct);
    }
}
