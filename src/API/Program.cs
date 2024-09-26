using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

//file:///C:/ASPNetCore/modular-monolith-eshop/src/API/bin/Debug/net8.0/Catalog.dll
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

app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception is null) return;

            var problemDetails = new ProblemDetails()
            {
                Title = exception.Message,
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.StackTrace?.TrimStart()
            };

            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, exception.Message);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        });
    }
);

app.Run();