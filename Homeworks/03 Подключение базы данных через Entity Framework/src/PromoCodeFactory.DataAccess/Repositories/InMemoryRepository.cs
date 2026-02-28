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
        if (_data.TryGetValue(id, out var result))
            return Task.FromResult((T?)result);
        else
            return Task.FromResult((T?)null);
    }

    public Task Add(T entity, CancellationToken ct)
    {
        _data.TryAdd(entity.Id, entity);
        return Task.CompletedTask;
    }

    public Task Update(T entity, CancellationToken ct)
    {
        if (!_data.TryGetValue(entity.Id, out var existing))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        if (!_data.TryUpdate(entity.Id, entity, existing))
            throw new InvalidOperationException(
                $"Failed to update {typeof(T).Name} with Id '{entity.Id}'.");

        return Task.CompletedTask;
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        if (!_data.TryRemove(id, out _))
            throw new EntityNotFoundException(typeof(T), id);

        return Task.CompletedTask;
    }
}
