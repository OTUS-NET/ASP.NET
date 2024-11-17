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
            var dataList = Data as List<T>;
            dataList.Add(item);

            return Task.FromResult(item);
        }

        public Task<T> UpdateAsync(T item)
        {
            var dataList = Data as List<T>;

            var oldItem = Data.FirstOrDefault(x => x.Id == item.Id);

            if (oldItem is not null)
            {
                int index = dataList.IndexOf(oldItem);
                dataList[index] = item;

                return Task.FromResult(item);
            }
            else
            {
                throw new InvalidOperationException("По 'id' не найден объект для обновления данных!");
            }
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            var item = Data.FirstOrDefault(x => x.Id == id);

            if (item is not null)
            {
                var dataList = Data as List<T>;
                dataList.Remove(item);

                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false); // значит с таким id не найдено!
            }           
        }
    }
}