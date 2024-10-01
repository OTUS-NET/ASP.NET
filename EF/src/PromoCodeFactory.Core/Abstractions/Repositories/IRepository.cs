using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken token);
        Task<T> GetByIdAsync(Guid id, CancellationToken token);
        Task CreateAsync(T entity, CancellationToken token);
        Task UpdateAsync(Guid id, T entity, CancellationToken token);
        Task DeleteByIdAsync(Guid id, CancellationToken token);
    }
}