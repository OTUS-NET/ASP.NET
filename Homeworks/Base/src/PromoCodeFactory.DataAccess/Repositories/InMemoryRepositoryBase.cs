using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepositoryBase<T>: IRepository<T> where T: BaseEntity
    {
        protected IDictionary<Guid, T> Data { get; set; }

        public InMemoryRepositoryBase(IDictionary<Guid, T> data)
        {
            Data = data;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(Data.Values.AsEnumerable());
        }

        public virtual Task<T> GetByIdAsync(Guid id)
        {
            if (!Data.ContainsKey(id))
                return Task.FromResult<T>(null);

            return Task.FromResult(Data[id]);
        }

        public virtual Task<T> CreateAsync(T entity)
        {
            Guid id = Guid.NewGuid();
            entity.Id = id;
            Data[id] = entity;
            return Task.FromResult(entity);
        }

        public virtual Task<T> UpdateAsync(T entity)
        {
            if (!Data.ContainsKey(entity.Id))
                return Task.FromResult<T>(null);

            Data[entity.Id] = entity;
            return Task.FromResult(entity);
        }

        public virtual Task<Guid> DeleteAsync(Guid entityId)
        {
            if (!Data.ContainsKey(entityId))
                return Task.FromResult(Guid.Empty);

            Data.Remove(entityId);

            return Task.FromResult(entityId);
        }

        
    }
}