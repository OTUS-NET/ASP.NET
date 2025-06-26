using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using Pcf.Administration.Core.Domain;

namespace Pcf.Administration.Core.Abstractions.Repositories
{
    public interface IRepository<T>
        where T: BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<T> GetByIdAsync(ObjectId id);
        
        Task<IEnumerable<T>> GetRangeByIdsAsync(List<ObjectId> ids);
        
        Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate);
        
        Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}