using Pcf.Common.Contracts.GivingToCustomer;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using System.Net.Http.Json;

namespace Pcf.ReceivingFromPartner.Integration.GivingToCustomer;

public class GivingToCustomerGateway(HttpClient httpClient) : IGivingToCustomerGateway
{
    /// <inheritdoc/>
    public async Task CreatePromoCode(PromoCode promoCode, CancellationToken ct)
    {
        var request = new CreatePromoCodeMessage(
            promoCode.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.Partner.Id,
            promoCode.PreferenceId);

        using var response = await httpClient.PostAsJsonAsync("api/v1/promocodes", request, ct);
        response.EnsureSuccessStatusCode();
    }
}
