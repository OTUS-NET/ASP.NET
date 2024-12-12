using MassTransit;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.WebHost.Exseption;
using Pcf.ReceivingFromPartner.Message;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromoCodeAdministrationEventConsumer(IRepository<Employee> repository) : IConsumer<PromoCodeMessage>
    {
        public async Task Consume(ConsumeContext<PromoCodeMessage> context)
        {
            var promoCode = context.Message;
            var employee = await repository.GetByIdAsync(promoCode.PreferenceId) ??
                 throw new NotFoundException(Comment.FormatNotFoundErrorMessage(promoCode.PreferenceId, "Preference"));

            employee.AppliedPromocodesCount++;
            await repository.UpdateAsync(employee);
        }
    }
}
