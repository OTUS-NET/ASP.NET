using Pcf.Common.Core.Domain;
using Pcf.Common.Core.Exceptions;
using System.Linq.Expressions;

namespace Pcf.Common.Core.Abstractions.Repositories;

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
}
