namespace Pcf.ReceivingFromPartner.Core.Abstractions;

public interface IAdministrationGateway
{
    /// <summary>
    /// Сотрудник существует и имеет роль партнерского менеджера.
    /// </summary>
    Task<bool> IsPartnerManager(Guid employeeId, CancellationToken ct);
}
