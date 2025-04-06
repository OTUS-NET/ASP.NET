using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Repositories;

public class PreferenceRepository : IRepository<Preference>
{
    DatabaseContext _context;
    public PreferenceRepository(DatabaseContext context)
    {
        _context = context;
    }


    public async Task AddAsync(Preference entity, CancellationToken cancellationToken)
    {
        await _context.Preferences.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Delete(Preference entity)
    {
        _context.Preferences.Remove(entity);
        _context.SaveChanges();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var Preference = await _context.Preferences.FindAsync(id, cancellationToken);

        if (Preference == null)
            return false;

        _context.Preferences.Remove(Preference);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<Preference> GetAll(bool asNoTracking)
    {
        return asNoTracking
            ? _context.Preferences.AsNoTracking()
            : _context.Preferences;
    }

    public async Task<IEnumerable<Preference>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking)
    {
        return asNoTracking
            ? await _context.Preferences.AsNoTracking().ToListAsync(cancellationToken)
            : await _context.Preferences.ToListAsync(cancellationToken);
    }

    public async Task<Preference> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Preferences.FindAsync(id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Update(Preference entity)
    {
        _context.Preferences.Update(entity);
        _context.SaveChanges();
    }

    public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        var Preference = await _context.Preferences.FindAsync(id, cancellationToken);

        if (Preference == null)
            return false;

        _context.Preferences.Update(Preference);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddRangeIfNotExistsAsync(IEnumerable<Preference> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            var existingEntity = await _context.Preferences.FindAsync(entity.Id, cancellationToken);
            if (existingEntity == null)
            {
                await AddAsync(entity, cancellationToken);
            }
        }
    }
}
