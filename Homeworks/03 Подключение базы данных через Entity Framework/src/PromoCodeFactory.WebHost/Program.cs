using PromoCodeFactory.DataAccess;
var builder = WebApplication.CreateBuilder();

builder.Services.AddInMemoryDataAccess();

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
