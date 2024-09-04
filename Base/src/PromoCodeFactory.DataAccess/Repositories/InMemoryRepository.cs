using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
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

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(T entity)
        {
            Data.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateByIdAsync(Guid id, T entity)
        {
            if (!Data.Any(x => x.Id == id)) throw new ArgumentException("Id is not valid");
            var index = Data.IndexOf(Data.FirstOrDefault(x => x.Id == id));
            entity.Id = id;
            Data[index] = entity;

            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync(Guid id)
        {
            var entry = Data.FirstOrDefault(e => e?.Id == id);
            if (entry != null)
            {
                Data.Remove(entry);
            }
            return Task.CompletedTask;
        }
    }
}