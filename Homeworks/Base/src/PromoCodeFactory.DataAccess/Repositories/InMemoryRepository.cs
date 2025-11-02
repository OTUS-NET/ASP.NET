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
        protected static List<T> Data = new();

        public InMemoryRepository(IEnumerable<T> data)
        {
            if(!Data.Any() && data != null)
            {
                Data = data.ToList();
            }
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T entity)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));
            entity.Id = Guid.NewGuid();
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var index = Data.FindIndex(x => x.Id == entity.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Сущность с таким {entity.Id} не найдена");
            Data[index] = entity;
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Guid id)
        {
            var entity = Data.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                throw new KeyNotFoundException($"Сущность с таким {id} не найдена");
            Data.Remove(entity);
            return Task.CompletedTask;
        }
    }
}