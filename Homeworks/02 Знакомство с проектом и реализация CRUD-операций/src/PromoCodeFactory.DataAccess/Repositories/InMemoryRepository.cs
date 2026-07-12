using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Exceptions;
using System.Collections.Concurrent;

namespace PromoCodeFactory.DataAccess.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ConcurrentDictionary<Guid, T> _data;

    public InMemoryRepository(IEnumerable<T> data)
    {
        _data = new ConcurrentDictionary<Guid, T>(data.Select(e => new KeyValuePair<Guid, T>(e.Id, e)));
    }
    public Task<IReadOnlyCollection<T>> GetAll(CancellationToken ct)
    {
        return Task.FromResult((IReadOnlyCollection<T>)_data.Values);
    }

    public Task<T?> GetById(Guid id, CancellationToken ct)
    {
        _data.TryGetValue(id, out var result);
        return Task.FromResult(result);
    }

    public Task Add(T entity, CancellationToken ct)
    {
        _data.TryAdd(Guid.NewGuid(), entity);
        return Task.FromResult(entity);
    }

    public Task Update(T entity, CancellationToken ct)
    {
        if (!_data.TryGetValue(entity.Id, out var value))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        _data.TryUpdate(entity.Id, entity, value);
        return Task.FromResult(entity);
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        if (!_data.TryRemove(id, out var entity) && entity == null)
            throw new EntityNotFoundException(typeof(T), id);

        return Task.FromResult(entity);
    }
}
