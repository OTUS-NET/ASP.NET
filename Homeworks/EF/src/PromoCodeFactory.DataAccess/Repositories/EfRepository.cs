using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        public DbContext Db { get; set; }
        protected readonly DbSet<T> _data;

        public EfRepository(DbContext context)
        {
            Db = context ?? throw new ArgumentNullException(nameof(context));
            _data = Db.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Func<T, bool> condition = null)
        {
            IEnumerable<T> query = _data;
            if (condition != null)
                query = query.Where(condition);

            return await Task.FromResult(query.ToList());
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _data.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                entity.Id = Guid.NewGuid();
                var result = await _data.AddAsync(entity);
                await Db.SaveChangesAsync();

                return result.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении сущности: {ex.Message}");
                return null;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                Db.Attach(entity);
                Db.Entry(entity).State = EntityState.Modified;
                var result = _data.Update(entity);
                var affectedRows = await Db.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    Console.WriteLine($"Ошибка при обновлении сущности");
                }

                return result.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении сущности: {ex.Message}");
                return null;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _data.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                {
                    _data.Remove(entity);
                    await Db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении сущности: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранить изменения.
        /// </summary>
        public virtual void SaveChanges()
        {
            Db.SaveChanges();
        }

        /// <summary>
        /// Сохранить изменения.
        /// </summary>
        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Db.SaveChangesAsync(cancellationToken);
        }
    }
}