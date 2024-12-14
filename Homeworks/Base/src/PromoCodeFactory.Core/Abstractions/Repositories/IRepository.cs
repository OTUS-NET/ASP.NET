using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IList<T>> GetAllAsync();

        Task<T> GetAsync(Guid id);

        Task<T> AddAsync(T entity);

        Task<T> UpdateAsync(T entity);
        
        Task<T> DeleteAsync(Guid id);
    }
}