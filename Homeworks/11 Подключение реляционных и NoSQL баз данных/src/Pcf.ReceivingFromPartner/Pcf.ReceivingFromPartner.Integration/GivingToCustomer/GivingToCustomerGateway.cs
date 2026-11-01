using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Domain;
using System.Net.Http.Json;

namespace Pcf.ReceivingFromPartner.Integration.GivingToCustomer;

public class GivingToCustomerGateway(HttpClient httpClient) : IGivingToCustomerGateway
{
    /// <inheritdoc/>
    public async Task CreatePromoCode(PromoCode promoCode, CancellationToken ct)
    {
        var request = new CreatePromoCodeRequest
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            ServiceInfo = promoCode.ServiceInfo,
            BeginDate = promoCode.BeginDate,
            EndDate = promoCode.EndDate,
            PartnerId = promoCode.Partner.Id,
            PreferenceId = promoCode.Preference.Id,
        };

        using var response = await httpClient.PostAsJsonAsync("api/v1/promocodes", request, ct);
        response.EnsureSuccessStatusCode();
    }
}
