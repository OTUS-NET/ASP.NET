using Infrastructure.RabbitMq;

namespace Pcf.GivingToCustomer.WebHost.Settings
{
    public class ApplicationSettings
    {
        public RmqSettings RmqSettings { get; set; }
    }
}