using PromoCodeFactory.Core.Abstractions.Services;
using PromoCodeFactory.Core.Services;
using PromoCodeFactory.DataAccess;
var builder = WebApplication.CreateBuilder();

builder.Services.AddDataAccess();

builder.Services.AddProblemDetails();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddControllers();

builder.Services.AddOpenApi(builder.Environment);
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<IRoleService, RoleService>();
var app = builder.Build();

app.UseExceptionHandler();

app.MapOpenApi();
app.MapSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
