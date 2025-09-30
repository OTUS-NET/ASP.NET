using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
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

        public Task<T?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        /// <inheritdoc />
        public Task<T> AddAsync(T customer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<T> UpdateAsync(T customer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task RemoveAsync(T customer)
        {
            throw new NotImplementedException();
        }
    }
}