using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories.EF
{
    public class EfRepositoryBase<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> _entitySet;
        public EfRepositoryBase( DbContext dbContext)
        {
            Context = dbContext;
            _entitySet = Context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Func<T, bool> condition = null)
        {
            IEnumerable<T> query = _entitySet;
            if (condition != null)
                query = query.Where(condition);
            
            return await Task.FromResult(query.ToList());
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _entitySet.FirstAsync(x => x.Id == id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            T result = null;
            var existing = await _entitySet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            //DetachLocal(existing, existing.Id);

            if (existing != null)
            {
                result = _entitySet.Update(entity).Entity;
                Context.SaveChanges();
            }
               
            else
            {
                //Минимальная валидация
                Console.WriteLine($"Error: (Update) Entity with id {entity.Id} does not exist");
            }
            return result;
        }

        public async Task<T> CreateAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            var addResult = await _entitySet.AddAsync(entity);
            T result = addResult.Entity;
            Context.SaveChanges();
            return result;
        }

        public async Task<Guid> DeleteAsync(Guid entityId)
        {
            Guid result = Guid.Empty;
            var existing = await _entitySet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entityId);
            //DetachLocal(existing, existing.Id);

            if (existing != null)
            {
                result = _entitySet.Remove(existing).Entity.Id;
            }
            else
            {
                //Минимальная валидация
                Console.WriteLine($"Error: (Delete) Entity with id {entityId} does not exist");
            }
            Context.SaveChanges();
            return result;
        }

        protected void DetachLocal(T t, Guid entryId)
        {
            var local = Context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));
            if (local != null)
            {
                Context.Entry(local).State = EntityState.Detached;
            }
            Context.Entry(t).State = EntityState.Modified;
        }
    }
}
