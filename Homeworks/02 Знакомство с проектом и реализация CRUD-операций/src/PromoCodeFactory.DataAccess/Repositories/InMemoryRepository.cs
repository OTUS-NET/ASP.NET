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
        if (_data.TryGetValue(id, out T? value))
        {
            return Task.FromResult(value);
        }

        throw new EntityNotFoundException(typeof(T), id);
    }

    public Task Add(T entity, CancellationToken ct)
    {
        if (_data.TryAdd(entity.Id, entity))
        {
            return Task.CompletedTask;
        }
        else
        {
            return Task.FromCanceled(ct);
        }
    }

    public Task Update(T entity, CancellationToken ct)
    {
        if (!_data.TryGetValue(entity.Id, out _))
        {
            throw new EntityNotFoundException(typeof(T), entity.Id);
        }


        if (_data.TryUpdate(entity.Id, entity, _data[entity.Id]))
        {
            return Task.CompletedTask;
        }
        else
        {
            return Task.FromCanceled(ct);
        }
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        if (!_data.TryGetValue(id, out _))
        {
            throw new EntityNotFoundException(typeof(T), id);
        }

        if (_data.TryRemove(id, out _))
        {
            return Task.CompletedTask;
        }
        else
        {
            return Task.FromCanceled(ct);
        }
    }
}
