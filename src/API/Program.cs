var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();

builder.Services
    .AddCatalogModule(config)
    .AddBasketModule(config)
    .AddOrderingModule(config);

var app = builder.Build();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

//configure the HTTP request pipeline
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.Run();