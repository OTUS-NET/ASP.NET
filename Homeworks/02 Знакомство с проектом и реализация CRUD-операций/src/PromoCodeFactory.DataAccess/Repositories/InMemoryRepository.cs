using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;
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
        _data.TryGetValue(id, out T? value);

        return Task.FromResult(value);
    }

    public Task Add(T entity, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (!_data.TryAdd(entity.Id, entity))
            throw new InvalidOperationException($"Entity with Id {entity.Id} already exists");

        return Task.CompletedTask;
    }

    public Task Update(T entity, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (!_data.TryGetValue(entity.Id, out var currentValue))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        if (!_data.TryUpdate(entity.Id, entity, currentValue))
            throw new InvalidOperationException( $"Cannot update {typeof(T).Name} with id {entity.Id}.");

        return Task.CompletedTask;
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        if (!_data.TryRemove(id, out _))
            throw new EntityNotFoundException(typeof(T), id);

        return Task.CompletedTask;
    }
}
