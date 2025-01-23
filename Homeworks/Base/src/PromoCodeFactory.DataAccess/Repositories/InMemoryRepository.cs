using PromoCodeFactory.Core.Abstractions.Entities;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected List<T> Data { get; set; }
        protected IEntityHepler<T> EntityHepler { get; set; }

        public InMemoryRepository(
            IEnumerable<T> data, 
            IEntityHepler<T> entityHepler) : 
            this(data)
        {
            EntityHepler = entityHepler;
        }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data?.ToList() ?? new();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<T> CreateAsync(T item)
        {
            item.Id = Guid.NewGuid();
            Data.Add(item);
            return Task.FromResult(item);
        }

        public Task<T> UpdateAsync(T item)
        {
            var actual = Data.FirstOrDefault(x => x.Id == item.Id);
            actual = EntityHepler?.UpdateEntity(actual, item);
            return Task.FromResult(actual);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await GetByIdAsync(id);

            if (item != default)
            {
                var isOk = Data.Remove(item);
                return await Task.FromResult(isOk);
            }

            return await Task.FromResult(true);
        }
    }
}