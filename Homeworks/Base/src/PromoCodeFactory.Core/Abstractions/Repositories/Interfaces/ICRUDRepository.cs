using PromoCodeFactory.Core.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories.Interfaces
{
    public interface ICRUDRepository<T, TCreate, TUpdate> : IRepository<T> where T : BaseEntity
    {
        Task<T> AddAsync(TCreate entity, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(Guid entityId, TUpdate entity, CancellationToken cancellationToken = default);

        Task<Guid> DeleteAsync(Guid entityId, CancellationToken cancellationToken = default);
    }
}
