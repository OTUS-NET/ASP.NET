using MediatR;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Common.Exceptions;
using PromoCodeFactory.Contracts.Customers;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Customers;

public class UpdateCustomerCommand : IRequest
{
    public required Guid Id { get; set; }
    
    public required CustomerSetDto Data { get; set; } 
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly PromoCodesDbContext _dbContext;

    public UpdateCustomerCommandHandler(PromoCodesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FindAsync([request.Id], cancellationToken: cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException("Customer not found");
        }

        if (await _dbContext.Customers.AnyAsync(
                x => x.Email == request.Data.Email.ToLower(),
                cancellationToken) &&
            !customer.Email.ToLower().Equals(request.Data.Email, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new ArgumentException("Customer with this email already exists");
        }

        customer.FirstName = request.Data.FirstName;
        customer.LastName = request.Data.LastName;
        customer.Email = request.Data.Email;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}