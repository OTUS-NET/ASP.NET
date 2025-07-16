using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure.RabbitMq;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using RabbitMQ.Client.Events;

namespace Pcf.GivingToCustomer.WebHost.Services.Promocodes;

public class PromocodesService : RabbitMqConsumer, IPromocodesService
{
    private readonly IRepository<PromoCode> _promoCodesRepository;
    private readonly IRepository<Preference> _preferencesRepository;
    private readonly IRepository<Customer> _customersRepository;

    public PromocodesService(IRepository<PromoCode> promoCodesRepository, IRepository<Preference> preferencesRepository, IRepository<Customer> customersRepository)
    {
        _promoCodesRepository = promoCodesRepository;
        _preferencesRepository = preferencesRepository;
        _customersRepository = customersRepository;
    }

    protected override async Task OnConsumerOnReceivedAsync(object sender, BasicDeliverEventArgs e)
    {
        var body = e.Body;
        var message = JsonSerializer.Deserialize<GivePromoCodeToCustomerDto>(Encoding.UTF8.GetString(body.ToArray()));
        
        var preference = await _preferencesRepository.GetByIdAsync(message!.PreferenceId);

        if (preference == null)
        {
            return;
        }

        //  Получаем клиентов с этим предпочтением:
        var customers = await _customersRepository
            .GetWhere(d => d.Preferences.Any(x =>
                x.Preference.Id == preference.Id));

        PromoCode promoCode = PromoCodeMapper.MapFromModel(message, preference, customers);

        await _promoCodesRepository.AddAsync(promoCode);
    }
}