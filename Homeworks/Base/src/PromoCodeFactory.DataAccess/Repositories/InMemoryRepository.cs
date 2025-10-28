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

        public Task<bool> RemoveAsync(Guid id)
        {
            var list = Data.ToList();
            var removeCount = list.RemoveAll(x => x.Id == id);
            Data = list;
            return Task.FromResult(removeCount > 0);
        }

        public Task<Guid> AddAsync(T data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));

            data.Id = Guid.NewGuid();
            Data = Data.Append(data);
            
            return Task.FromResult(data.Id);
        }

        public Task<T> UpdateAsync(T data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));
            
            var list = Data.ToList();
            var index = list.FindIndex(x => x.Id == data.Id);
            
            if(index < 0)
                throw new KeyNotFoundException(data.Id.ToString());
            
            list[index] = data;
            Data = list;
            
            return Task.FromResult(data);
        }
    }
}