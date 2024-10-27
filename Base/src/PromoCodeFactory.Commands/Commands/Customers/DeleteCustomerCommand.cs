using MediatR;
using PromoCodeFactory.Common.Exceptions;
using PromoCodeFactory.DataAccess;

namespace PromoCodeFactory.Commands.Commands.Customers;

public class DeleteCustomerCommand : IRequest
{
    public required Guid Id { get; set; }
}

public class DeleteCustomerCommandHandler(PromoCodesDbContext dbContext) : IRequestHandler<DeleteCustomerCommand>
{
    private readonly PromoCodesDbContext _dbContext = dbContext;

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .FindAsync([request.Id, cancellationToken], cancellationToken: cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException("Customer not found");
        }

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}