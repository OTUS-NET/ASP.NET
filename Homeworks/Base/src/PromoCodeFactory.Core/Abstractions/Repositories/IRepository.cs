using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;


namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<T>> CreateAsync(IEnumerable<T> empl, CancellationToken cancellationToken);

        Task<T> ReplaceAsync(IEnumerable<T> empl, Guid id, CancellationToken cancellationToken);
    }
}