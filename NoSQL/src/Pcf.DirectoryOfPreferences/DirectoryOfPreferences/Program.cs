using DirectoryOfPreferences;
using DirectoryOfPreferences.Application.Implementations.Mapping;
using DirectoryOfPreferences.ExceptionHandling;
using DirectoryOfPreferences.Infrastructure.EntityFramework;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDataContext(builder.Configuration);
builder.Services.AddRepository().AddServices();
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(Program), typeof(PreferenceMapping));

builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "PromoCode Factory Directory Of Preference API Doc";
    options.Version = "1.0";
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});


builder.Services.AddFluentValidationAutoValidation()
                .AddValidators();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseOpenApi();
app.UseSwaggerUi(x =>
{
    x.DocExpansion = "list";
});

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.MigrateDatabase<DataContext>();
app.UseErrorHandler();

app.Run();
