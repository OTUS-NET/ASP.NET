using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T, Guid>
        where T : BaseEntity
    {
        protected ICollection<T> Data { get; set; }

        public InMemoryRepository(ICollection<T> data)
        {
            Data = data;
        }

        public Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            Data.Add(entity);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public IQueryable<T> GetAll(bool asNoTracking = false)
        {
            return Data.AsQueryable();
        }

        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false)
        {
            return Task.FromResult((IEnumerable<T>)Data);
        }

        public Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public void Update(T entity) { }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await GetAsync(id, cancellationToken);
            return entity != null && Data.Remove(entity);
        }

        public void Delete(T entity)
        {
            Data.Remove(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}