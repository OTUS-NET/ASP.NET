using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

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
            return Task.FromResult<IEnumerable<T>>(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task DeleteAsync(Guid id)
        {
            Data = Data.Where(x => x.Id != id).ToList();
            return Task.CompletedTask;
        }
        public Task CreateAsync(T entity)
        {
            Data.Add(entity);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(T entity)
        {
            var index = Data.FindIndex(x => x.Id == entity.Id);
            if (index != -1)
            {
                Data[index] = entity;
            }
            return Task.CompletedTask;
        }
    }
}