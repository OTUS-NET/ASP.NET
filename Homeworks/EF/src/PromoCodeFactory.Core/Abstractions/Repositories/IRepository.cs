using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetRangeByIdsAsync(IList<Guid> ids);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}