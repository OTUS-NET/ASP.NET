using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.Contracts;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Abstractions.Services.Models;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromoCodeReceivedFromPartnerEventConsumer : IConsumer<PromoCodeReceivedFromPartnerEvent>
    {
        private readonly IPromoCodeIssuingService _promoCodeIssuingService;

        public PromoCodeReceivedFromPartnerEventConsumer(IPromoCodeIssuingService promoCodeIssuingService)
        {
            _promoCodeIssuingService = promoCodeIssuingService;
        }

        public async Task Consume(ConsumeContext<PromoCodeReceivedFromPartnerEvent> context)
        {
            var message = context.Message;

            var ok = await _promoCodeIssuingService.GivePromoCodesToCustomersWithPreferenceAsync(
                new GivePromoCodeToCustomersWithPreferenceCommand
                {
                    PromoCodeId = message.PromoCodeId,
                    PartnerId = message.PartnerId,
                    PromoCode = message.PromoCode,
                    PreferenceId = message.PreferenceId,
                    BeginDate = message.BeginDate,
                    EndDate = message.EndDate,
                    ServiceInfo = message.ServiceInfo
                });

            if (!ok)
                throw new InvalidOperationException($"Preference '{message.PreferenceId}' was not found");
        }
    }
}

