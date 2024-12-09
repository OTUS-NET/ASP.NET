using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data) => Data = data.ToList();

        public Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult(Data);

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Data.FirstOrDefault(x => x.Id == id));

        public Task<T> CreateAsync(T obj, CancellationToken cancellationToken = default)
        {
            obj.Id = Guid.NewGuid();
            Data.Add(obj);
            return Task.FromResult(obj);
        }

        public Task<T> UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
            Data.RemoveAll(x => x.Id == obj.Id);
            Data.Add(obj);
            return Task.FromResult(obj);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var count = Data.Count;
            Data.RemoveAll(x => x.Id == id);
            return Task.FromResult(Data.Count < count);
        }
    }
}