using System.Linq.Expressions;
using Pcf.Common.Core.Abstractions.Repositories;
using Pcf.Common.Core.Exceptions;
using Pcf.ReceivingFromPartner.Core.Domain;
using StackExchange.Redis;

namespace Pcf.ReceivingFromPartner.DataAccess.Repositories;

internal class PreferenceRedisRepository(IConnectionMultiplexer redis) : IRepository<Preference>
{
    public const string Key = "preferences";

    private IDatabase Database => redis.GetDatabase();

    public async Task<IReadOnlyCollection<Preference>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var entries = await Database.HashGetAllAsync(Key);
        return entries.Select(ToPreference).ToArray();
    }

    public async Task<Preference?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var name = await Database.HashGetAsync(Key, Field(id));
        if (name.IsNullOrEmpty)
            return null;

        return new Preference { Id = id, Name = name! };
    }

    public async Task<IReadOnlyCollection<Preference>> GetByRangeId(
        IEnumerable<Guid> ids,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var idList = ids.Distinct().ToArray();
        var names = await Database.HashGetAsync(Key, idList.Select(id => (RedisValue)Field(id)).ToArray());

        return idList
            .Zip(names)
            .Where(pair => !pair.Second.IsNullOrEmpty)
            .Select(pair => new Preference { Id = pair.First, Name = pair.Second! })
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Preference>> GetWhere(
        Expression<Func<Preference, bool>> predicate,
        bool withIncludes = false,
        CancellationToken ct = default)
    {
        var preferences = await GetAll(withIncludes, ct);
        return preferences.Where(predicate.Compile()).ToArray();
    }

    public async Task Add(Preference entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        await Database.HashSetAsync(Key, Field(entity.Id), entity.Name);
    }

    public async Task Update(Preference entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (!await Database.HashExistsAsync(Key, Field(entity.Id)))
            throw new EntityNotFoundException<Preference>(entity.Id);

        await Database.HashSetAsync(Key, Field(entity.Id), entity.Name);
    }

    public async Task Delete(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (!await Database.HashDeleteAsync(Key, Field(id)))
            throw new EntityNotFoundException<Preference>(id);
    }

    private static string Field(Guid id) => id.ToString("D");

    private static Preference ToPreference(HashEntry entry)
    {
        return new Preference
        {
            Id = Guid.Parse(entry.Name.ToString()),
            Name = entry.Value.ToString()
        };
    }
}
