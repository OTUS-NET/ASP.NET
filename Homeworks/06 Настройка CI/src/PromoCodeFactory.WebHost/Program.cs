using PromoCodeFactory.DataAccess;

var builder = WebApplication.CreateBuilder();

builder.Services.AddEfDataAccess();

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
