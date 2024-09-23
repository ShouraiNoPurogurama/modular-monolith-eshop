var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//Add services to the container
// builder.Services.AddCarter(configurator: config =>
// {
//     //get all types that implements ICarterModule in CatalogModule assembly
//     var catalogModules = typeof(CatalogModule).Assembly.GetTypes()
//         .Where(t => t.IsAssignableTo(typeof(ICarterModule)))
//         .ToArray();
//
//     config.WithModules(catalogModules);
// });
builder.Services.AddCarterWithAssemblies(typeof(CatalogModule).Assembly);

builder.Services.AddControllers();

builder.Services
    .AddCatalogModule(config)
    .AddBasketModule(config)
    .AddOrderingModule(config);

var app = builder.Build();

app.MapCarter();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

//configure the HTTP request pipeline
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.Run();