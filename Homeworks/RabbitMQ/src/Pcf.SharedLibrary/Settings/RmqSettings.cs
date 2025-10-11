namespace Pcf.SharedLibrary.Settings
{
    public class RmqSettings
    {
        public required string Host { get; set; }
        public required string VHost { get; set; } = "/";
        public required string Login { get; set; }
        public required string Password { get; set; }
    }
}
