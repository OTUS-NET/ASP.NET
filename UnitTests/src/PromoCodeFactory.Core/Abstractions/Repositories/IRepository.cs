using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.Base;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<TEntity, in TId> 
        where TEntity : IEntity<TId> 
        where TId : struct
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TId id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(TId id,TEntity entity);  
        Task DeleteAsync(TId id);   
    }
}