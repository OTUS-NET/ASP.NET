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
        protected ICollection<T> Data { get; set; }

        public InMemoryRepository(ICollection<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult((IEnumerable<T>)Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<Guid> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            Data.Add(entity);
            return Task.FromResult(entity.Id);
        }

        public Task<bool> RemoveAsync(Guid id)
        {
            var entity = Data.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(Data.Remove(entity));
        }
    }
}