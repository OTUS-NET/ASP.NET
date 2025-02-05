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

        public Task<T> CreateAsync(T item)
        {
            item.Id = Guid.NewGuid();
            Data = Data.Append(item);
            return Task.FromResult(item);
        }

        public Task<T> UpdateAsync(Guid id, T item)
        {
            var itemToUpdate = Data.First(x => x.Id == id);
            var list = Data.ToList();
            var updateItemIndex = list.IndexOf(itemToUpdate);
            list[updateItemIndex] = item;
            Data = list.AsEnumerable();
            return Task.FromResult(item);
        }

        public Task DeleteAsync(Guid id)
        {
            var list = Data.ToList();
            var itemToDelete = Data.First(x => x.Id == id);
            list.Remove(itemToDelete);
            Data = list.AsEnumerable();
            return Task.CompletedTask;
        }
    }
}