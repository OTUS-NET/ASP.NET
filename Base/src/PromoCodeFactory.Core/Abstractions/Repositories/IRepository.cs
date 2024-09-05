using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();
    }
}