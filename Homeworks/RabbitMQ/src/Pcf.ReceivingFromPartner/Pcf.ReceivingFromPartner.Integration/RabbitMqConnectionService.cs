using Microsoft.Extensions.Options;
using Pcf.Integration;
using Pcf.SharedLibrary.Settings;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class RabbitMqConnectionService(IOptions<ApplicationSettings> options) : RabbitMqConnectionServiceBase(options)
    {
    }
}
