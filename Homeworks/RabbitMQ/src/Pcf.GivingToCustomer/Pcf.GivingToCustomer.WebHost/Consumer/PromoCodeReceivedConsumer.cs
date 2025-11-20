using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.IntegrationEvents;

namespace Pcf.GivingToCustomer.WebHost.Consumer;

public class PromoCodeReceivedConsumer(IGivingToCustomerService givingToCustomerService) : IConsumer<PromoCodeIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PromoCodeIntegrationEvent> context)
    {
        PromoCode promoCode = PromoCodeMapper.MapFromModel(context.Message); 

        await givingToCustomerService.GiveToCustomer(promoCode);
    }
}

