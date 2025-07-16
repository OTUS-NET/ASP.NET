using Infrastructure.RabbitMq;

namespace Pcf.ReceivingFromPartner.WebHost.Settings
{
    public class ApplicationSettings
    {
        public RmqSettings RmqSettings { get; set; }
    }
}