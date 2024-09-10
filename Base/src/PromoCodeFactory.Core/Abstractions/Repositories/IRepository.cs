using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<T> CreateAsync(CancellationToken cancellationToken = default);

        Task<bool> AddAsync(T item, CancellationToken cancellationToken = default);

        Task<bool> RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T obj, CancellationToken cancellationToken = default);
    }
}