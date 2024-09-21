using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task CreateAsync(T entity)
        {
            Data.Add(entity);
            
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            var existEntity = Data.FirstOrDefault(x => x.Id == entity.Id);

            if (existEntity is not null)
            {
                var index = Data.IndexOf(existEntity);
                Data[index] = entity;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var existEntity = Data.FirstOrDefault(x => x.Id == id);

            if (existEntity is not null)
            {
                Data.Remove(existEntity);
            }

            return Task.CompletedTask;
        }
    }
}