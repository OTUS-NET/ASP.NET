using PromoCodeFactory.DataAccess;
var builder = WebApplication.CreateBuilder();

var services = builder.Services;

services.AddDataAccess();
services.AddProblemDetails();
services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
services.AddControllers();
services.AddOpenApi(builder.Environment);

// Tune Custom ValidationFailed
services.ConfigureValidationFailed();

var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
