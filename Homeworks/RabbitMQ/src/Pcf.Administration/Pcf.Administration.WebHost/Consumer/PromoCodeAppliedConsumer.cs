using System.Threading.Tasks;
using MassTransit;
using Pcf.Administration.Core.Abstractions;
using Pcf.IntegrationEvents;

namespace Pcf.Administration.WebHost.Consumer;

public class PromoCodeAppliedConsumer(IPromoCodeService promoCodeService) : IConsumer<PartnerManagerIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PartnerManagerIntegrationEvent> context)
    {
        await promoCodeService.IncrementAppliedPromoCodesCountAsync(context.Message.PartnerId);
    }
}

