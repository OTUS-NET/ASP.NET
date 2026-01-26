using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories;

public interface IRepository<T> where T: BaseEntity
{
    Task<IReadOnlyCollection<T>> GetAll(CancellationToken ct);
}
