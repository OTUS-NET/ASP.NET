using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<List<T>> GetListByIdsAsync(IEnumerable<Guid> ids)
        {
            var result = new List<T>();
            foreach (var id in ids)
            {
                var entity = await GetByIdAsync(id)
                           ?? throw new EntityNotFoundException(id, typeof(T).Name);

                result.Add(entity);
            }

            return result;
        }

        public Task<T> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            Data.Add(entity);

            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(Guid id, T entity)
        {
            var entityIndex = Data.FindIndex(x => x.Id == id);

            if (entityIndex == -1)
                return Task.FromResult((T)null);

            entity.Id = id;

            Data[entityIndex] = entity;

            return Task.FromResult(entity);
        }

        public Task DeleteByIdAsync(Guid id)
        {
            var entity = Data.FirstOrDefault(x => x.Id == id)
                       ?? throw new EntityNotFoundException(id, typeof(T).Name);

            Data.Remove(entity);

            return Task.CompletedTask;
        }
    }
}
