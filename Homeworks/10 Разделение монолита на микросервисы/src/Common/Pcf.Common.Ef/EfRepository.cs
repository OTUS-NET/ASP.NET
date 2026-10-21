using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Domain;
using Pcf.Common.Core.Exceptions;

namespace Pcf.Common.Ef;

public class EfRepository<TDbContext, TEntity>(TDbContext context) : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : BaseEntity
{
    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query) => query;

    public async Task<IReadOnlyCollection<TEntity>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<TEntity>().AsQueryable();
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.ToListAsync(ct);
    }

    public async Task<TEntity?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<TEntity>().Where(e => e.Id == id);
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<TEntity>> GetByRangeId(
        IEnumerable<Guid> ids,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        var query = context.Set<TEntity>().Where(e => ids.Contains(e.Id));
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<TEntity>> GetWhere(
        Expression<Func<TEntity, bool>> predicate,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        var query = context.Set<TEntity>().Where(predicate);
        if (withIncludes)
            query = ApplyIncludes(query);
        return await query.ToListAsync(ct);
    }

    public async Task Add(TEntity entity, CancellationToken ct)
    {
        await context.Set<TEntity>().AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task Update(TEntity entity, CancellationToken ct)
    {
        var rowAffected = await context.SaveChangesAsync(ct);

        if(rowAffected == 0)
            throw new EntityNotFoundException(typeof(TEntity), entity.Id);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var rowAffected = await context
            .Set<TEntity>()
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(ct);

        if (rowAffected == 0)
            throw new EntityNotFoundException(typeof(TEntity), id);
    }
}
