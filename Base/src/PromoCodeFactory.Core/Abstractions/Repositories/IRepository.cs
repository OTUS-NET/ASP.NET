using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IList<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        void RemoveByIdAsync(Guid id);

        Task<T> UpdateByIdAsync(Guid id, T employee);

        Task<T> Add(T employee);
    }
}