using StackExchange.Redis;

namespace Pcf.PreferencesCache
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
                                                          ConnectionMultiplexer.Connect("my-redis-stack:6379, defaultDatabase=13"));

            // for local
            //builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            //                                  ConnectionMultiplexer.Connect("localhost:6669, defaultDatabase=1"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
