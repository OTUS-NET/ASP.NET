namespace PromoCodeFactory.Services.Repositories.Abstractions;

public interface ICustomerUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    IPreferenceRepository PreferenceRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}