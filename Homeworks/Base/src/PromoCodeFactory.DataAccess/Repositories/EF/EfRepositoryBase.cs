using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories.EF
{
    public abstract class EfRepositoryBase<T> : IRepository<T> where T : BaseEntity
    {
        public abstract Task<T> CreateAsync(T entity);
        public abstract Task<Guid> DeleteAsync(Guid entityId);
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(Guid id);
        public abstract Task<T> UpdateAsync(T entity);
    }
}
