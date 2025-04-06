using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task AddRangeIfNotExistsAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default);
        IQueryable<T> GetAll(bool asNoTracking);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);
        void Update(T entity);
        Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken = default);
        void Delete(T entity);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}