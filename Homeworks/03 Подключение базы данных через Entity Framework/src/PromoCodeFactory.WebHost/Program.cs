using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.DataAccess.Data;

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

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PromoCodeFactoryDbContext>();
    context.Database.Migrate();
    await PromoCodeFactoryDbSeeder.SeedAsync(context, CancellationToken.None);
}

app.UseExceptionHandler();

app.MapOpenApi();
app.MapSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
