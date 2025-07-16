namespace Pcf.Infrastructure.RabbitMq
{    
    public class RmqSettings
    {
        public string Host { get; set; }
        public string VHost { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}