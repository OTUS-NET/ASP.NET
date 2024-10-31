using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T: BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(List<T> data)
        {
            Data = data;
        }
        
        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            return Task.FromResult(Data.Where(x => ids.Contains(x.Id)).AsEnumerable());
        }

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}