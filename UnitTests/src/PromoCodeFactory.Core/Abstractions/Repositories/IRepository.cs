using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain.Base;
using System.Linq.Expressions;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    /// <summary>
    /// Previous
    /// </summary>
  
    //public interface IRepository<T> where T : BaseEntity
    //{
    //    Task<IEnumerable<T>> AllAsync { get; }

    //    Task<T> GetByIdAsync(Guid id);
    //    Task<T> CreateAsync(T entity);
    //    Task<T> UpdateAsync(Guid id, T entity);
    //    Task DeleteAsync(Guid id);
    //}
    public interface IRepository<TEntity, in TId>
       where TEntity : IEntity<TId>
       where TId : struct
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TId id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(TId id, TEntity entity);
        Task DeleteAsync(TId id);
    }
}