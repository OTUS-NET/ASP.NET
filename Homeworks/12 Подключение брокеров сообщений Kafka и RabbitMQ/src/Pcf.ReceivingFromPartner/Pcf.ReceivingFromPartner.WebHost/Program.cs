using Pcf.ReceivingFromPartner.DataAccess;
using Pcf.ReceivingFromPartner.Integration;
using Pcf.Common.Web;

var builder = WebApplication.CreateBuilder();

builder.Services
    .AddEfDataAccess()
    .AddIntegration();

builder.Services.AddProblemDetails();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddControllers();

builder.Services.AddOpenApi(builder.Environment);

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapSwaggerUI();

app.MapControllers();

app.MigrateDatabase();

if (app.Environment.IsDevelopment())
    await app.SeedDatabase();

app.Run();
