using System.Linq.Expressions;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class EfRepository<T>(PromoCodeFactoryDbContext context) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query) => query;

    public Task Add(T entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<T>> GetAll(bool withIncludes = false, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetById(Guid id, bool withIncludes = false, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<T>> GetByRangeId(IEnumerable<Guid> ids, bool withIncludes = false, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<T>> GetWhere(Expression<Func<T, bool>> predicate, bool withIncludes = false, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(T entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

}
