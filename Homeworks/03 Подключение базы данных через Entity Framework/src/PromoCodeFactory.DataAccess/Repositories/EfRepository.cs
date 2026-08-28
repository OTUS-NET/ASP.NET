using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories;

/// <summary>
/// Базовая реализация репозитория на Entity Framework Core.
/// Работает с любой доменной сущностью, которая наследуется от "BaseEntity"
/// </summary>
internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Добавляет выражения для навигационных свойств.
    /// В базовой реализации ничего не подключает; специализированные репозитории переопределяют метод.
    /// </summary>
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    /// <summary>
    /// Add - добавляет новую сущность в DbSet и сохраняет изменения в базе данных.
    /// </summary>
    public async Task Add(T entity, CancellationToken ct)
    {
        await context.Set<T>().AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Delete - удаляет сущность по идентификатору.
    /// Если сущность не найдена, выбрасывает "EntityNotFoundException">.
    /// </summary>
    public async Task Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.Set<T>().FindAsync([id], ct);
        if (entity is null)
            throw new EntityNotFoundException(typeof(T), id);

        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// GetAll - возвращает все сущности.
    /// Если withIncludes равен true, дополнительно загружает навигационные свойства.
    /// </summary>
    public async Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        var query = GetQuery(withIncludes);
        return await query.ToListAsync(ct);
    }

    /// <summary>
    /// GetById - возвращает одну сущность по идентификатору или null, если запись не найдена.
    /// Если withIncludes равен true, дополнительно загружает навигационные свойства.
    /// </summary>
    public async Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = GetQuery(withIncludes);
        return await query.FirstOrDefaultAsync(entity => entity.Id == id, ct);
    }

    /// <summary>
    /// GetByRangeId - возвращает все сущности, идентификаторы которых входят в переданный список.
    /// Используется, когда нужно загрузить несколько связанных записей одним запросом.
    /// </summary>
    public async Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        var idsArray = ids.ToArray();
        var query = GetQuery(withIncludes);
        return await query.Where(entity => idsArray.Contains(entity.Id)).ToListAsync(ct);
    }

    /// <summary>
    /// GetWhere - возвращает сущности, которые удовлетворяют переданному LINQ-предикату.
    /// Предикат выполняется на стороне базы данных, если EF Core может перевести его в SQL.
    /// </summary>
    public async Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        var query = GetQuery(withIncludes);
        return await query.Where(predicate).ToListAsync(ct);
    }

    /// <summary>
    /// Update - обновляет существующую сущность и сохраняет изменения.
    /// Перед обновлением проверяет, что запись с таким идентификатором существует.
    /// </summary>
    public async Task Update(T entity, CancellationToken ct)
    {
        if (!await context.Set<T>().AnyAsync(existing => existing.Id == entity.Id, ct))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        context.Set<T>().Update(entity);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// GetQuery - формирует базовый лист для DbSet.
    /// По флагу withIncludes подключает навигационные свойства через ApplyIncludes.
    /// </summary>
    private IQueryable<T> GetQuery(bool withIncludes)
    {
        var query = context.Set<T>().AsQueryable();
        return withIncludes ? ApplyIncludes(query) : query;
    }
}
