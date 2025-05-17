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
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(Data.AsEnumerable());
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public async Task<T> AddAsync(T entity)
        {
            Data.Add(entity);
            return await Task.FromResult(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var entityToUpdate = Data.FirstOrDefault(x => x.Id == entity.Id);
            if (entityToUpdate is not null)
            {
                int index = Data.IndexOf(entityToUpdate);
                Data[index] = entity;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T entity)
        {
            Data.Remove(entity);
            await Task.CompletedTask;
        }
    }
}