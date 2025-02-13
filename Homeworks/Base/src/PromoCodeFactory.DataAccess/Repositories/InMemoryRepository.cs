using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories;

public class InMemoryRepository<T>(IEnumerable<T> data) : IRepository<T> where T : BaseEntity
{
    protected List<T> Data { get; set; } = new List<T>(data);

    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult((IEnumerable<T>)Data);
    }

    public Task<T> GetByIdAsync(Guid id)
    {
        return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
    }

    public Task CreateEntityAsync(T entity)
    {
        Data.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteEntityAsync(T entity)
    {
        Data.Remove(entity);            
        return Task.CompletedTask;
    }

}