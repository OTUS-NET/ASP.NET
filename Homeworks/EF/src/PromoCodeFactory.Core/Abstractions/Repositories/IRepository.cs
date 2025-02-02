using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(Func<T, bool> condition = null);

        Task<T> GetByIdAsync(Guid id);

        Task<T> AddAsync(T customer);

        Task<T> UpdateAsync(T customer);

        Task DeleteAsync(Guid id);
    }
}