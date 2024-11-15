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

        public Task<T> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            Data=Data.Append(entity);
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
            Data = Data.Where(i => i.Id != entity.Id).AsEnumerable();
            Data= Data.Append(entity);
            return Task.FromResult(entity);
        }

        public Task<T> DeleteAsync(Guid id)
        {
            var existed = Data.FirstOrDefault(x => x.Id == id);
            Data = Data.Where(i => i.Id != id).AsEnumerable();
            return Task.FromResult(existed);
        }

    }
}