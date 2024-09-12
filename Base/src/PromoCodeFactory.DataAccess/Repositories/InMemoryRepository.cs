using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using static System.Reflection.Metadata.BlobBuilder;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IList<T> Data { get; set; }

        public InMemoryRepository(IList<T> data)
        {
            Data = data;
        }

        public Task<IList<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            var result = Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
            return result;
        }

        public Task<T> Add(T entity)
        {
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public void RemoveByIdAsync(Guid id)
        {
            Data.RemoveAt(Data.IndexOf(Data.FirstOrDefault(u => u.Id == id)));
        }

        public Task<T> UpdateByIdAsync(Guid id, T employee)
        {
            Data.RemoveAt(Data.IndexOf(Data.FirstOrDefault(u => u.Id == id)));
            Data.Add(employee);
            return Task.FromResult(employee);
        }
    }
}