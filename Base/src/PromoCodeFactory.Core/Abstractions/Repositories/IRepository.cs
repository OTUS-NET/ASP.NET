using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IList<T>> GetAllAsync(CancellationToken cts);
        Task<T> GetByIdAsync(Guid id, CancellationToken cts);
        Task AddAsync(T entity, CancellationToken cts);
        Task UpdateByIdAsync(Guid id, T entity, CancellationToken cts);
        Task DeleteByIdAsync(Guid id, CancellationToken cts);
        Task<bool> Exists(Guid id);
    }
}