using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        protected IList<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }


        public async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            return await Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(IList<Guid> ids)
        {
            var entities = Data.Where(x => ids.Contains(x.Id));
            return await Task.FromResult(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(Data);
        }

        public async Task<T> AddAsync(T entity)
        {
            Data.Add(entity);
            return await Task.FromResult(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var entityToUpdate = Data.FirstOrDefault(x => x.Id == entity.Id);
            if (entityToUpdate is not null)
            {
                int index = Data.IndexOf(entityToUpdate);
                Data[index] = entity;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T entity)
        {
            Data.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(Data.AsQueryable().FirstOrDefault(predicate));
        }
    }
}