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
        protected object lockObj = new object();
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
            Monitor.Enter(lockObj);

            entity.Id = Guid.NewGuid();
            IEnumerable<T> enumerable = Data.Concat(new[] { entity });
            Data = enumerable;

            Monitor.Exit(lockObj);

            return Task.FromResult(entity);
        }

        public Task DeleteAsync(Guid id)
        {
            Monitor.Enter(lockObj);

            IEnumerable<T> enumerable = Data.Where(x => x.Id != id);
            Data = enumerable;

            Monitor.Exit(lockObj);
            return Task.FromResult(enumerable);
        }
    }
}