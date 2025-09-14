using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected ICollection<T> Data { get; set; }

        public InMemoryRepository(ICollection<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        /// <inheritdoc />
        public Task<T> AddAsync(T entity)
        {
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        /// <inheritdoc />
        public async Task<T> UpdateAsync(T entity)
        {
            var oldEntity = await GetByIdAsync(entity.Id);
            
            if(oldEntity == null)
                throw new Exception("Entity not found");
            
            await DeleteAsync(oldEntity);
            await AddAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public Task DeleteAsync(T entity)
        {
            Data.Remove(entity);
            return Task.CompletedTask;
        }
    }
}