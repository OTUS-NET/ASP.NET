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
        protected List<T> Data { get; }
        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> AddAsync(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            Data.Add(item);
            return Task.FromResult(item);
        }

        public Task<T> UpdateAsync(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            var dataItem = Data.FirstOrDefault(x => x.Id == item.Id);
            if (dataItem is null)
                throw new KeyNotFoundException($"DataItem with id= {item.Id} not found");

            var index = Data.IndexOf(dataItem);
            Data[index] = item;
            return Task.FromResult(item);
        }

        public Task<bool> RemoveAsync(Guid id)
        {
            var item = Data.FirstOrDefault(x => x.Id == id);
            if (item is null)
                return Task.FromResult(false);

            return Task.FromResult(Data.Remove(item));
        }
    }
}