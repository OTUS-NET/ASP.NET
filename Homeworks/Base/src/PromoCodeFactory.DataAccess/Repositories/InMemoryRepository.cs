using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IList<T> Data { get; set; }

        public InMemoryRepository(IList<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }
        
        public Task<T> CreateAsync(T entity)
        {
            Data.Add(entity);
            return Task.FromResult(entity); 
        }
        
        public Task<T> UpdateAsync(Guid id, T entity)
        {
            var existingEntity = Data.FirstOrDefault(x => x.Id == id);
            var index = Data.IndexOf(existingEntity);
            Data[index] = entity;

            return Task.FromResult(entity);
        }
        
        public Task DeleteAsync(Guid id)
        {
            var entityToRemove = Data.FirstOrDefault(x => x.Id == id);
            Data.Remove(entityToRemove);
            return Task.CompletedTask;
        }
    }
}