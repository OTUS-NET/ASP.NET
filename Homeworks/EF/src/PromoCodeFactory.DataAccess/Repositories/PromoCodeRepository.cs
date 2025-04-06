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

public class PromoCodeRepository : IRepository<PromoCode>
{
    DatabaseContext _context;
    public PromoCodeRepository(DatabaseContext context)
    {
        _context = context;
    }


    public async Task AddAsync(PromoCode entity, CancellationToken cancellationToken)
    {
        await _context.PromoCodes.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Delete(PromoCode entity)
    {
        _context.PromoCodes.Remove(entity);
        _context.SaveChanges();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var PromoCode = await _context.PromoCodes.FindAsync(id, cancellationToken);

        if (PromoCode == null)
            return false;

        _context.PromoCodes.Remove(PromoCode);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public IQueryable<PromoCode> GetAll(bool asNoTracking)
    {
        return asNoTracking
            ? _context.PromoCodes.AsNoTracking()
            : _context.PromoCodes;
    }

    public async Task<IEnumerable<PromoCode>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking)
    {
        return asNoTracking
            ? await _context.PromoCodes.AsNoTracking().ToListAsync(cancellationToken)
            : await _context.PromoCodes.ToListAsync(cancellationToken);
    }

    public async Task<PromoCode> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.PromoCodes.FindAsync(id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Update(PromoCode entity)
    {
        _context.PromoCodes.Update(entity);
        _context.SaveChanges();
    }

    public async Task<bool> UpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        var PromoCode = await _context.PromoCodes.FindAsync(id, cancellationToken);

        if (PromoCode == null)
            return false;

        _context.PromoCodes.Update(PromoCode);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddRangeIfNotExistsAsync(IEnumerable<PromoCode> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            var existingEntity = await _context.PromoCodes.FindAsync(entity.Id, cancellationToken);
            if (existingEntity == null)
            {
                await AddAsync(entity, cancellationToken);
            }
        }
    }
}
