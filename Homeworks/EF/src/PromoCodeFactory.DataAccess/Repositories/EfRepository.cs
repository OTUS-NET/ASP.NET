using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        private readonly DatabaseContext _dataContext;

        //private readonly DbSet<T> _entitySet;
        protected DbSet<T> Data { get; set; }


        //protected readonly DbContext Context { get; set; }

        public EfRepository(DatabaseContext dataContext)
        {
            _dataContext = dataContext;
            Data = _dataContext.Set<T>();
        }

        public async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Data;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            var entity = await query.FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(IList<Guid> ids)
        {
            var entities = await Data.Where(x => ids.Contains(x.Id)).ToListAsync();
            return entities;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await Data.ToListAsync();
            return entities;
        }

        public async Task<T> AddAsync(T entity)
        {
            var entityEntryAdded = await Data.AddAsync(entity);
            await _dataContext.SaveChangesAsync();
            return entityEntryAdded.Entity;
        }

        public async Task UpdateAsync(T entity)
        {
            Data.Update(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            Data.Remove(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Data.FirstOrDefaultAsync(predicate);
        }
    }
}
