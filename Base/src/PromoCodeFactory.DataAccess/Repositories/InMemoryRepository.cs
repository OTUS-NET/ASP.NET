using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected static object lockObj = new object();
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

        public Task<T> CreateAsync(T entity)
        {
            try
            {
                Monitor.Enter(lockObj);
                entity.Id = Guid.NewGuid();
                IEnumerable<T> enumerable = Data.Concat(new[] { entity });
                Data = enumerable;
            }
            finally { Monitor.Exit(lockObj); }

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Guid id)
        {
            try
            {
                Monitor.Enter(lockObj);
                IEnumerable<T> enumerable = Data.Where(x => x.Id != id);
                Data = enumerable;
            }
            finally { Monitor.Exit(lockObj); }

            return Task.FromResult(Data);
        }

        public Task<T> UpdateAsync(Guid id, T entity)
        {
            try
            {
                Monitor.Enter(lockObj);
                var date = Data.Where(x => x.Id != id).ToList();
                date.Add(entity);
                Data = date;
            }
            finally { Monitor.Exit(lockObj); }

            return Task.FromResult(entity);
        }
    }
}