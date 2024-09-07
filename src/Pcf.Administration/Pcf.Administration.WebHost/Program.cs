using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;

namespace Pcf.Administration.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}