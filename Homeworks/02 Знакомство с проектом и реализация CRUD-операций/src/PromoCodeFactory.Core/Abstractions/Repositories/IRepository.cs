using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.Core.Abstractions.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyCollection<T>> GetAll(CancellationToken ct);

    /// <summary>
    /// Returns employee by id or null if not found
    /// </summary>
    Task<T?> GetById(Guid id, CancellationToken ct);

    Task Add(T entity, CancellationToken ct);

    /// <exception cref="EntityNotFoundException"/>
    Task Update(T entity, CancellationToken ct);

    /// <exception cref="EntityNotFoundException"/>
    Task Delete(Guid id, CancellationToken ct);
}
