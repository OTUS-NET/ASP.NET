using System;
using System.Threading.Tasks;
using MassTransit;
using Pcf.Administration.Core.Abstractions.Services;
using Pcf.Contracts;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromoCodeReceivedFromPartnerEventConsumer : IConsumer<PromoCodeReceivedFromPartnerEvent>
    {
        private readonly IAppliedPromocodesService _appliedPromocodesService;

        public PromoCodeReceivedFromPartnerEventConsumer(IAppliedPromocodesService appliedPromocodesService)
        {
            _appliedPromocodesService = appliedPromocodesService;
        }

        public async Task Consume(ConsumeContext<PromoCodeReceivedFromPartnerEvent> context)
        {
            var employeeId = context.Message.PartnerManagerId;
            if (!employeeId.HasValue)
                return;

            var updated = await _appliedPromocodesService.IncrementAppliedPromocodesAsync(employeeId.Value);
            if (!updated)
                throw new InvalidOperationException($"Employee '{employeeId.Value}' was not found");
        }
    }
}

