using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PromoCodeFactory.Core;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid id);
        
        Task LoadRelatedDataAsync<TProperty>(T entity, Expression<Func<T, TProperty>> navigationProperty);
    }
}