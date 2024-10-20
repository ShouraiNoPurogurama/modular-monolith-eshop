var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (context, config) => { config.ReadFrom.Configuration(context.Configuration); });

var config = builder.Configuration;

//C:/ASPNetCore/modular-monolith-eshop/src/API/bin/Debug/net8.0/Catalog.dll
var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;

//Common services: carter, mediatr, fluentvalidation
builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, basketAssembly);

builder.Services.AddControllers();

//Module services: catalog, basket, ordering
builder.Services
    .AddCatalogModule(config)
    .AddBasketModule(config)
    .AddOrderingModule(config);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapCarter();
app.UseSerilogRequestLogging();
//Force the app to use custom exception handler
app.UseExceptionHandler(option => { });

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

//configure the HTTP request pipeline
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.Run();