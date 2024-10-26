using Keycloak.AuthServices.Authentication;
using Shared.Messaging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, config) => { config.ReadFrom.Configuration(context.Configuration); });

var config = builder.Configuration;

//C:/ASPNetCore/modular-monolith-eshop/src/API/bin/Debug/net8.0/Catalog.dll
var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;
var orderingAssembly = typeof(OrderingModule).Assembly;

//Common services: carter, mediatr, FluentValidation
builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, basketAssembly, orderingAssembly);

//Regis redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddMassTransitWithAssemblies(builder.Configuration ,catalogAssembly, basketAssembly);

builder.Services.AddControllers();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

//Module services: catalog, basket, ordering
builder.Services
    .AddCatalogModule(config)
    .AddBasketModule(config)
    .AddOrderingModule(config);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

//configure the HTTP request pipeline
app.MapCarter();
app.UseSerilogRequestLogging();
//Force the app to use custom exception handler
app.UseExceptionHandler(option => { });

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

app.Run();