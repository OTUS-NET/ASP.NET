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

        public Task<IList<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> GetAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> UpdateAsync(T entity)
        {
            var existed = Data.FirstOrDefault(x => x.Id == entity.Id);
            if (existed == null) return Task.FromResult(existed);
            Data.Remove(existed);
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> DeleteAsync(Guid id)
        {
            var existed = Data.FirstOrDefault(x => x.Id == id);
            if (existed!=null) Data.Remove(existed);
            return Task.FromResult(existed);
        }

    }
}