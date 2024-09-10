using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity, new()
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data) => Data = data.ToList();

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) => await Task.FromResult(Data);

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await Task.FromResult(Data.FirstOrDefault(x => x.Id == id));

        public async Task<T> CreateAsync(CancellationToken cancellationToken = default) 
        {
            T obj = new T();
            obj.Id = Guid.NewGuid();
            return await Task.FromResult(obj);
        }

        public async Task<bool> AddAsync(T item, CancellationToken cancellationToken = default)
        {
            bool contains = Data.Contains(item);
            if(!contains)
                Data.Add(item);
            return await Task.FromResult(contains);
        }

        public async Task<bool> RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            int countRemoved = Data.RemoveAll(x => x.Id == id);
            return await Task.FromResult(countRemoved > 0);
        }

        public async Task<T> UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
            await RemoveByIdAsync(obj.Id, cancellationToken);
            await AddAsync(obj, cancellationToken);
            return await Task.FromResult(obj);
        }
    }
}