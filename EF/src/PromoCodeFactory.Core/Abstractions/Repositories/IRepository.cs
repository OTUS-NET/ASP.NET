using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(T employee, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    }
}