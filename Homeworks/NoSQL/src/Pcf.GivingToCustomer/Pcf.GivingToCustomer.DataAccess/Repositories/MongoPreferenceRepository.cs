using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess.Mongo;

namespace Pcf.GivingToCustomer.DataAccess.Repositories
{
    public class MongoPreferenceRepository : IRepository<Preference>
    {
        private readonly IMongoCollection<Preference> _preferences;

        public MongoPreferenceRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
        {
            _preferences = database.GetCollection<Preference>(settings.Value.PreferencesCollectionName);
        }

        public async Task<IEnumerable<Preference>> GetAllAsync()
        {
            return await _preferences.Find(FilterDefinition<Preference>.Empty).ToListAsync();
        }

        public async Task<Preference> GetByIdAsync(Guid id)
        {
            return await _preferences.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Preference>> GetRangeByIdsAsync(List<Guid> ids)
        {
            return await _preferences.Find(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<Preference> GetFirstWhere(Expression<Func<Preference, bool>> predicate)
        {
            return await _preferences.AsQueryable().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Preference>> GetWhere(Expression<Func<Preference, bool>> predicate)
        {
            return await _preferences.AsQueryable().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Preference entity)
        {
            await _preferences.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Preference entity)
        {
            await _preferences.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = false });
        }

        public async Task DeleteAsync(Preference entity)
        {
            await _preferences.DeleteOneAsync(x => x.Id == entity.Id);
        }
    }
}

