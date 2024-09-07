using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(Guid id);

        Task<T> UpdateAsync(Guid id, T entity);

        Task<T> AddAsync(T entity);

        Task DeleteByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<List<T>> GetListByIdsAsync(IEnumerable<Guid> ids);
    }
}