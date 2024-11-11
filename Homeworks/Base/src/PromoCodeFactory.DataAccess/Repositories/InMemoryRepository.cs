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
        private readonly List<T> _data;

        public InMemoryRepository(IEnumerable<T> data)
        {
            _data = data.ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            _data.Add(entity);
            return Task.FromResult(entity);
        }
    }
}