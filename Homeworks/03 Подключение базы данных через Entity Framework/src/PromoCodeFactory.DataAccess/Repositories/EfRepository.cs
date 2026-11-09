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
        await context.Set<T>().AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.Set<T>().FindAsync([id], ct);

        if (entity == null)
            throw new EntityNotFoundException(typeof(T), id);
        
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().AsQueryable();
        query = withIncludes ? ApplyIncludes(query) : query;
        return await query.ToListAsync(ct);
    }

    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().AsQueryable();
        query = withIncludes ? ApplyIncludes(query) : query;
        return await query.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(w => ids.Contains(w.Id));
        query = withIncludes ? ApplyIncludes(query) : query;
        return await query.ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = context.Set<T>().Where(predicate);
        query = withIncludes ? ApplyIncludes(query) : query;
        return await query.ToListAsync(ct);
    }

    public async Task Update(T entity, CancellationToken ct)
    {
        var exists = await context.Set<T>().AnyAsync(x => x.Id == entity.Id, ct);
        if(!exists) throw new EntityNotFoundException(typeof(T), entity.Id);
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>Выполняет операцию в транзакции БД.</summary>
    /// <param name="action">Асинхронная операция с изменениями.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <remarks>
    /// При исключении внутри <paramref name="action"/> транзакция откатывается,
    /// а исключение пробрасывается вызывающему коду.
    /// </remarks>
    public async Task ExecuteInTransaction(Func<Task> action, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
