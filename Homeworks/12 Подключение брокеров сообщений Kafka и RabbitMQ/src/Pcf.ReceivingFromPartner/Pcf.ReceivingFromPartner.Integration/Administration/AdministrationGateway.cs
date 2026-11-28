using System.Net;
using System.Net.Http.Json;
using Pcf.ReceivingFromPartner.Core.Abstractions;

namespace Pcf.ReceivingFromPartner.Integration.Administration;

public class AdministrationGateway(HttpClient httpClient) : IAdministrationGateway
{
    private const string PartnerManagerRoleName = "PartnerManager";

    /// <inheritdoc/>
    public async Task<bool> IsPartnerManager(Guid employeeId, CancellationToken ct)
    {
        using var response = await httpClient.GetAsync($"api/v1/employees/{employeeId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();

        var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>(ct);
        return employee?.Role.Name == PartnerManagerRoleName;
    }
}
