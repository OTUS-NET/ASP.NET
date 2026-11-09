using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;
using System.Linq.Expressions;

namespace PromoCodeFactory.Core.Abstractions.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default);

    Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default);

    Task<IReadOnlyCollection<T>> GetByRangeId(
        IEnumerable<Guid> ids,
        bool withIncludes = false,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<T>> GetWhere(
        Expression<Func<T, bool>> predicate,
        bool withIncludes = false,
        CancellationToken ct = default);

    Task Add(T entity, CancellationToken ct);

    /// <exception cref="EntityNotFoundException"/>
    Task Update(T entity, CancellationToken ct);

    /// <exception cref="EntityNotFoundException"/>
    Task Delete(Guid id, CancellationToken ct);

    /// <summary>
    /// Выполняет указанную асинхронную операцию в рамках одной транзакции.
    /// Все изменения, произведённые внутри <paramref name="action"/>, фиксируются атомарно:
    /// при успехе — коммит, при исключении — откат.
    /// </summary>
    /// <param name="action">
    /// Асинхронная операция, содержащая изменения репозитория (например, несколько вызовов <c>Add</c>).
    /// </param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <remarks>
    /// Для хранилищ без поддержки транзакций (например, in-memory) операция просто выполняется
    /// без явного коммита/отката, но контракт остаётся единым.
    /// </remarks>
    Task ExecuteInTransaction(Func<Task> action, CancellationToken ct);
}
