using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected List<T> _data { get; set; }

        public InMemoryRepository(List<T> data)
        {
            _data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(_data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_data.FirstOrDefault(x => x.Id == id));
        }

        public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var items = await Task.Run(() => _data.Where(t => ids.Contains(t.Id)).Select(t => t).ToList());

            return items;
        }

        public async Task Create(T t)
        {
            await Task.Run(() => _data.Add(t));
        }

        public async Task Delete(T t)
        {
            await Task.Run(() => _data.Remove(t));
        }
    }
}