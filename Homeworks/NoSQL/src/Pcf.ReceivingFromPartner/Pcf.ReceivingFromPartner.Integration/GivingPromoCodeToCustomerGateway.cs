using System.Net.Http;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration.Dto;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class GivingPromoCodeToCustomerGateway
        : IGivingPromoCodeToCustomerGateway
    {
        private readonly HttpClient _httpClient;

        public GivingPromoCodeToCustomerGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task GivePromoCodeToCustomer(PromoCode promoCode)
        {
            var dto = new GivePromoCodeToCustomerDto()
            {
                PartnerId = promoCode.Partner.Id,
                BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
                PreferenceId = promoCode.PreferenceId,
                PromoCodeId = promoCode.Id,
                PromoCode = promoCode.Code,
                ServiceInfo = promoCode.ServiceInfo,
                PartnerManagerId = promoCode.PartnerManagerId
            };
            
            var response = await _httpClient.PostAsJsonAsync("api/v1/promocodes", dto);

            response.EnsureSuccessStatusCode();
        }
    }
}