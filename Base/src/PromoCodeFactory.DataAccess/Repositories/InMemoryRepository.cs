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

        public Task AddAsync(T entity)
        {
            (Data as List<T>).Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateByIdAsync(Guid id, T entity)
        {
            if ((Data as List<T>).Any(e => e.Id == id))
            {
                DeleteByIdAsync(id);
            }

            entity.Id = id;
            AddAsync(entity);

            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync(Guid id)
        {
            var entry = (Data as List<T>).FirstOrDefault(e => e?.Id == id);
            if (entry != null)
            {
                (Data as List<T>).Remove(entry);
            }
            return Task.CompletedTask;
        }
    }
}