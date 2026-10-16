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
        ct.ThrowIfCancellationRequested();

        return Task.FromResult((IReadOnlyCollection<T>)_data.Values);
    }

    public Task<T?> GetById(Guid id, CancellationToken ct)
    {
        return Task.FromResult(_data.Values.FirstOrDefault(x=>x.Id == id));
    }

    public Task<T> Add(T entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Guid id = Guid.NewGuid();
        entity.Id = id;
        _data.TryAdd(id, entity);
        return Task.FromResult(entity);
    }

    public Task Update(T entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!_data.TryUpdate(entity.Id, entity, _data[entity.Id]))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        return Task.CompletedTask;
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!_data.TryRemove(id, out _))
            throw new EntityNotFoundException(typeof(T) ,id);

        return Task.CompletedTask;
    }
}
