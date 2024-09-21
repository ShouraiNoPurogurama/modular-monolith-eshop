using Catalog.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<CatalogDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        
        return services;
    }

    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        //1. Use Api Endpoint services
        
        //2. Use Application Use Case services
        
        //3. Use Data - Infrastructure services
        app.UseMigration<CatalogDbContext>();
        
        return app;
    }

}