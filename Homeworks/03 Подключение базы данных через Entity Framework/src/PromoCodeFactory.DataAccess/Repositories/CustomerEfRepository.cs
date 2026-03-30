using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.Core.Exceptions;

namespace PromoCodeFactory.DataAccess.Repositories;

internal class CustomerEfRepository(PromoCodeFactoryDbContext context) : EfRepository<Customer>(context)
{
    protected override IQueryable<Customer> ApplyIncludes(IQueryable<Customer> query)
    {
        return query
            .Include(c => c.Preferences)
            .Include(c => c.CustomerPromoCodes);
    }

    public override async Task Update(Customer entity, CancellationToken ct)
    {
        var set = context.Set<Customer>();
        var entityToUpdate = await ApplyIncludes(set).FirstOrDefaultAsync(e => e.Id == entity.Id, ct);

        if (entityToUpdate is null)
        {
            throw new EntityNotFoundException(typeof(Customer), entity.Id);
        }
        var entityEntry = set.Entry(entityToUpdate);
        var newPreferences = set.Entry(entity).Collection(c => c.Preferences).CurrentValue;
        var newCustomerPormoCodes = set.Entry(entity).Collection(c => c.CustomerPromoCodes).CurrentValue;

        if(newPreferences is not null)
        {
            entityEntry.Collection(c => c.Preferences).CurrentValue = newPreferences;
        }
        if(newCustomerPormoCodes is not null)
        {
            entityEntry.Collection(c => c.CustomerPromoCodes).CurrentValue = newCustomerPormoCodes;
        }
        set.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(ct);
    }
}
