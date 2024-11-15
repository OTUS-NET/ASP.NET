using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;


namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> DeleteByIdAsync(Guid id);

        Task<IEnumerable<T>> CreateAsync(IEnumerable<T> empl);

        Task<T> ReplaceAsync(IEnumerable<T> empl, Guid id);
    }
}