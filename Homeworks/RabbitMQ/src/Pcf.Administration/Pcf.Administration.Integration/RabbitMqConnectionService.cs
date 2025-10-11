using Microsoft.Extensions.Options;
using Pcf.Integration;
using Pcf.SharedLibrary.Settings;

namespace Pcf.Administration.Integration
{
    public class RabbitMqConnectionService(IOptions<ApplicationSettings> options) : RabbitMqConnectionServiceBase(options)
    {
    }
}
