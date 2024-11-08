using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories;

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

    public Task<T> CreateAsync(T entity)
    {
        Data = Data.Append(entity);
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        var index = Data.TakeWhile(e =>  e.Id != entity.Id).Count();
        Data = Data.Take(index).Append(entity).Concat(Data.TakeLast(Data.Count() - index - 1));
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var index = Data.TakeWhile(e => e.Id != id).Count();
        Data = Data.Take(index).Concat(Data.TakeLast(Data.Count() - index - 1));
        return Task.FromResult(true);
    }
}