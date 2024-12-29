using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.Core.PromoCodeManagement;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Customers;

public class AddCustomerPreferenceCommand : IRequest
{
    public required CustomerPreferenceDto Data { get; set; }
}

public class AddCustomerPreferenceCommandHandler(PromoCodesDbContext dbContext) : IRequestHandler<AddCustomerPreferenceCommand>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task Handle(AddCustomerPreferenceCommand request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
                           .Include(x => x.CustomerPreferences)
                           .FirstOrDefaultAsync(x => x.Id == request.Data.CustomerId, cancellationToken)
                       ?? throw new ArgumentException("Покупатель не найден");

        var preference = await _dbContext.Preferences
                             .FirstOrDefaultAsync(x => x.Id == request.Data.PreferenceId, cancellationToken)
                         ?? throw new ArgumentException("Предпочтение не найдено");

        if (customer.CustomerPreferences.Any(x => x.PreferenceId == preference.Id))
        {
            throw new ArgumentException("Данное предпочтение уже добавлено покупателю");
        }

        var customerPreference = new CustomerPreference
        {
            CustomerId = customer.Id,
            PreferenceId = preference.Id,
        };

        await _dbContext.CustomerPreferences.AddAsync(customerPreference, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}