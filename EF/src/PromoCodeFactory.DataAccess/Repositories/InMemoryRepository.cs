using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Base;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : struct
    {
        protected static object lockObj = new object();
        protected IEnumerable<TEntity> Data { get; set; }

        public InMemoryRepository(IEnumerable<TEntity> data)
        {
            Data = data;
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<TEntity> GetByIdAsync(TId id)
        {
            return Task.FromResult(Data.FirstOrDefault(predicate: x => x.Id.Equals(id)));
        }

        public Task<TEntity> CreateAsync(TEntity  entity)
        {
            try
            {
                Monitor.Enter(lockObj);
                IEnumerable<TEntity> enumerable = Data.Concat(new[] { entity });
                Data = enumerable;
            }
            finally { Monitor.Exit(lockObj); }

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(TId id)
        {
            try
            {
                Monitor.Enter(lockObj);
                IEnumerable<TEntity> enumerable = Data.Where(predicate: x => x.Id.Equals(id));
                Data = enumerable;
            }
            finally { Monitor.Exit(lockObj); }

            return Task.FromResult(Data);
        }

        public Task UpdateAsync(TId id, TEntity entity)
        {
            try
            {
                Monitor.Enter(lockObj);
                var date = Data.Where(predicate: x => x.Id.Equals(id)).ToList();
                date.Add(entity);
                Data = date;
            }
            finally { Monitor.Exit(lockObj); }        
            return Task.CompletedTask;
        }
    }
}