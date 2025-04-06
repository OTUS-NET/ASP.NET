using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
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

        Task IRepository<T>.AddAsync(T entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<T> IRepository<T>.GetAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        IQueryable<T> IRepository<T>.GetAll(bool asNoTracking)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<T>> IRepository<T>.GetAllAsync(CancellationToken cancellationToken, bool asNoTracking)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<T>.UpdateAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<T>.DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Delete(T entity)
        {
            throw new NotImplementedException();
        }

        Task IRepository<T>.SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeIfNotExistsAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}