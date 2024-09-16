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

        public Task AddAsync(T entity)
        {
            (Data as List<T>).Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateByIdAsync(Guid id, T item)
        {
            
            DeleteByIdAsync(id);
            

            item.Id = id;
            AddAsync(item);

            return Task.CompletedTask;
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            var item = (Data as List<T>).FirstOrDefault(e => e?.Id == id);
            if (item== null)
                return Task.FromResult(false);

            (Data as List<T>).Remove(item);
            return Task.FromResult(true);
        }
    }
}