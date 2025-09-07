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
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T entity)
        {
            Data = Data.Append(entity);
            return Task.FromResult(entity);
        }
        
        public Task<T> UpdateAsync(Guid id, T entity)
        { 
            var item = Data.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return null;
            }
            
            var index = Data.ToList().IndexOf(item);
            Data.ToList()[index] = entity;
            return Task.FromResult(entity);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var item = Data.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return Task.FromResult(false);
            }
            
            Data = Data.Where(x => x.Id != id);
            return Task.FromResult(true);
        }
    }
}