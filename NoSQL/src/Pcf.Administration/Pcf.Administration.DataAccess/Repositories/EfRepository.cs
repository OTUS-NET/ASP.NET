using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain;

namespace Pcf.Administration.DataAccess.Repositories
{
    public class EfRepository<T>
        : IRepository<T>
        where T: BaseEntity
    {
        private readonly DataContext _dataContext;

        public EfRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await _dataContext.Set<T>().ToListAsync();
            DetectChangesToConsole();
            return entities;
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var entity = await _dataContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            DetectChangesToConsole();
            return entity;
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<string> ids)
        {
            var entities = await _dataContext.Set<T>().Where(x => ids.Contains(x.Id)).AsNoTracking().ToListAsync();
            DetectChangesToConsole();
            return entities;
        }

        public async Task<T> GetFirstWhere(Expression<Func<T, bool>> predicate)
        {
            DetectChangesToConsole();
            return await _dataContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            DetectChangesToConsole();
            return await _dataContext.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            DetectChangesToConsole();
            await _dataContext.Set<T>().AddAsync(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            DetectChangesToConsole();
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dataContext.Set<T>().Remove(entity);
            await _dataContext.SaveChangesAsync();
        }

        private void DetectChangesToConsole()
        {
            _dataContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_dataContext.ChangeTracker.DebugView.LongView);
        }
    }
}