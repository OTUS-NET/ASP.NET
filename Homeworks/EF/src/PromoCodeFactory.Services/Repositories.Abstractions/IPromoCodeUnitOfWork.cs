namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface IPromoCodeUnitOfWork
{
    IPromoCodeRepository PromoCodeRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IPreferenceRepository PreferenceRepository { get; }
    IEmployeeRepository EmployeeRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}