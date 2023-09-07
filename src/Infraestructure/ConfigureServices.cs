using Application.Common.Interfaces;
using Domain.Interfaces;
using Infraestructure.Common.Caching;
using Infraestructure.Common.Enums;
using Infraestructure.Common.Interfaces;
using Infraestructure.Files;
using Infraestructure.Health;
using Infraestructure.Persistence;
using Infraestructure.Persistence.Base;
using Infraestructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure;

public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            //.AddSqlServer(configuration.GetConnectionString("NorthwindConnection")!);
            .AddPingHealthCheck(opt =>
            {
                opt.AddHost("192.168.18.10",1);
            } )
            .AddCheck<SqlHealthCheck>("NorthwindDatabase");
        
        ConfigureDapper(services, configuration);
        ConfigureBuilders(services);
        ConfigureDbContext(services, configuration);
        ConfigureDistributedCache(services,configuration);
        ConfigureRepositories(services);
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddTransient<ICustomerRepository, CustomerRepository>();
    }
    private static void ConfigureDapper(IServiceCollection services, IConfiguration configuration)
    {
        var connectionDict = new Dictionary<DatabaseConnectionName, string?>
        {
            { DatabaseConnectionName.NorthwindDbConnection, configuration.GetConnectionString("NorthwindConnection") }
        };
        services.AddSingleton<IDictionary<DatabaseConnectionName, string?>>(connectionDict);
        services.AddTransient<IDbConnectionFactory, DapperDbConnectionFactory>();
    }
    private static void ConfigureBuilders(IServiceCollection services)
    {
        services.AddScoped<ICsvFileBuilder, CsvFileBuilder>();
        services.AddScoped<IExcelFileBuilder, ExcelFileBuilder>();
    }
    private static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NorthwindContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("NorthwindConnection")));
        services.AddTransient<INorthwindContext, NorthwindContext>();
    }
    private static void ConfigureDistributedCache(IServiceCollection services,IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();
        services.AddTransient<ICacheService, MemoryCacheService>();
        services.Configure<CacheConfiguration>(configuration.GetSection("CacheConfig"));
        //Documentación
        //https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0
        /*services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("MyRedisConStr");
            options.InstanceName = "SampleInstance";
        });*/
    }
}