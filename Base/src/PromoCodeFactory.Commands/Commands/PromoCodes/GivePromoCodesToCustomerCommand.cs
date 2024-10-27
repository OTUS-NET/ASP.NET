using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts.PromoCodes;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.PromoCodes;

public class GivePromoCodesToCustomerCommand : IRequest<int>
{
    public required GivePromoCodeRequestDto Data { get; set; }
}

public class GivePromoCodesToCustomerCommandHandler(PromoCodesDbContext dbContext)
    : IRequestHandler<GivePromoCodesToCustomerCommand, int>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task<int> Handle(GivePromoCodesToCustomerCommand request, CancellationToken cancellationToken)
    {
        var preference = await _dbContext.Preferences.FindAsync([request.Data.PreferenceId], cancellationToken);

        if (preference == null)
        {
            throw new ArgumentException("Preference not found");
        }

        var eligibleCustomers = await _dbContext.Customers
            .Include(c => c.CustomerPreferences)
            .Where(c => c.CustomerPreferences.Any(cp => cp.PreferenceId == request.Data.PreferenceId))
            .ToListAsync(cancellationToken);

        if (!eligibleCustomers.Any())
        {
            throw new ArgumentException("There are no eligible customers");
        }

        var promoCodesCreated = 0;

        foreach (var promoCode in eligibleCustomers.Select(customer => new PromoCode
                 {
                     Code = request.Data.PromoCode + "_" + Guid.NewGuid().ToString("N")[..8],
                     ServiceInfo = request.Data.ServiceInfo,
                     BeginDate = request.Data.BeginDate,
                     EndDate = request.Data.EndDate,
                     PartnerName = request.Data.PartnerName,
                     PreferenceId = request.Data.PreferenceId,
                     CustomerId = customer.Id
                 }))
        {
            _dbContext.PromoCodes.Add(promoCode);
            promoCodesCreated++;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return promoCodesCreated;
    }
}