using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.Core.Abstractions.Services
{
    /// <summary>
    /// Сигнатура та же, что в IRepository, однако предполагается, что IRepository заменяет нативные методы EF Core, так что считаю решение приемлимым. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IService<T> where T: BaseEntity
    {
        Task<IList<T>> GetAllAsync(CancellationToken cts);
        Task<T> GetByIdAsync(Guid id, CancellationToken cts);
        Task AddAsync(T entity, CancellationToken cts);
        Task UpdateByIdAsync(Guid id, T entity, CancellationToken cts);
        Task DeleteByIdAsync(Guid id, CancellationToken cts);
    }
}
