using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T, TPrimaryKey>
        where T : IEntity<TPrimaryKey>
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);
        Task<T> GetAsync(TPrimaryKey id, CancellationToken cancellationToken);
        IQueryable<T> GetAll(bool asNoTracking = false);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);
        Task<bool> UpdateAsync(TPrimaryKey id, CancellationToken cancellationToken);
        void Update(T entity);
        Task<bool> DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken);
        void Delete(T entity);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}