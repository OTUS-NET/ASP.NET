using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var oldData = Data.ToList();
            oldData.Add(entity);
            Data = oldData;

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(T entity)
        {
            var oldData = Data.ToList();
            oldData.Remove(entity);
            Data = oldData;

            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            var oldData = Data.ToList();
            var index = oldData.FindIndex(x => x.Id == entity.Id);
            if (index != -1)
            {
                oldData[index] = entity;
                Data = oldData; 
            }

            return Task.CompletedTask;
        }
    }
}