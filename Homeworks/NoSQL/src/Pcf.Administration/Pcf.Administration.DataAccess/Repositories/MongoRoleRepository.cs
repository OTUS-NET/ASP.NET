using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Microsoft.Extensions.Options;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.DataAccess.Mongo;

namespace Pcf.Administration.DataAccess.Repositories
{
    public class MongoRoleRepository : IRepository<Role>
    {
        private readonly IMongoCollection<Role> _roles;

        public MongoRoleRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
        {
            _roles = database.GetCollection<Role>(settings.Value.RolesCollectionName);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roles.Find(FilterDefinition<Role>.Empty).ToListAsync();
        }

        public async Task<Role> GetByIdAsync(Guid id)
        {
            return await _roles.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Role>> GetRangeByIdsAsync(List<Guid> ids)
        {
            return await _roles.Find(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<Role> GetFirstWhere(Expression<Func<Role, bool>> predicate)
        {
            return await _roles.AsQueryable().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Role>> GetWhere(Expression<Func<Role, bool>> predicate)
        {
            return await _roles.AsQueryable().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Role entity)
        {
            await _roles.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Role entity)
        {
            await _roles.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        }

        public async Task DeleteAsync(Role entity)
        {
            await _roles.DeleteOneAsync(x => x.Id == entity.Id);
        }
    }
}

