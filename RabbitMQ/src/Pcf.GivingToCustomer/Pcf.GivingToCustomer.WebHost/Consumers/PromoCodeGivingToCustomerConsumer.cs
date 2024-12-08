using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;
using Pcf.GivingToCustomer.WebHost.Settings.Exceptions;
using Pcf.ReceivingFromPartner.Message;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromoCodeGivingToCustomerConsumer(IRepository<Core.Domain.PromoCode> promoCodesRepository,
            IRepository<Core.Domain.Preference> preferencesRepository,
            IRepository<Customer> customersRepository) : IConsumer<PromoCodeMessage>
    {
        public async Task Consume(ConsumeContext<PromoCodeMessage> context)
        {
            var promoCodeMessage = context.Message;

            var request = new GivePromoCodeRequest()
            {
                PartnerId = promoCodeMessage.PartnerId,
                BeginDate = promoCodeMessage.BeginDate.ToShortDateString(),
                EndDate = promoCodeMessage.EndDate.ToShortDateString(),
                PreferenceId = promoCodeMessage.PreferenceId,
                PromoCode = promoCodeMessage.Code,
                ServiceInfo = promoCodeMessage.ServiceInfo,
            };

            var preference = await preferencesRepository.GetByIdAsync(request.PreferenceId) ??
                throw new BadRequestException(Comment.FormatBadRequestErrorMessage(request.PreferenceId, nameof(Preference)));

            //  Получаем клиентов с этим предпочтением:
            var customers = await customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            Core.Domain.PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preference, customers);

            await promoCodesRepository.AddAsync(promoCode);
        }
    }
}
