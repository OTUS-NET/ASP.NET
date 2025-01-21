using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);
        
        Task<T> CreateAsync(T item);

        Task<T> UpdateAsync(T item);

        Task<bool> DeleteAsync(Guid id);
    }
}