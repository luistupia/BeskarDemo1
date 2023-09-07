using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Versioning;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using WebApi.Filters;
using WebApi.Middlewares;

namespace WebApi;

public class Startup
{
    public static WebApplication InitializeApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder);
        var app = builder.Build();
        Configure(app);
        return app;
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        ConfigureSwagger(builder);
        ConfigureNewtonSoft(builder);
        ConfigureSerilog(builder);
        ConfigureCors(builder);
        ConfigureServicesLayers(builder);
        AddAttributesAndFilters(builder);
    }

    private static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("mycors");
        app.UseMiddleware<ExceptionMiddleware>();
        
        app.MapHealthChecks("/_health_check", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
    private static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        //Serilog
        //https://github.com/serilog/serilog/wiki/Enrichment
        var logger = new LoggerConfiguration()
            //.ReadFrom.Configuration(builder.Configuration) //Para leer configuracion del appsettings.json
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.File("..\\Logs\\LOG-.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: false,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {CorrelationId} {Level:u3} {Username} {Message:lj}{Exception}{NewLine}")
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
    }
    private static void ConfigureNewtonSoft(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddNewtonsoftJson(x =>
        {
            x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            x.SerializerSettings.Formatting = Formatting.Indented;
            x.SerializerSettings.ContractResolver = new DefaultContractResolver();
        });
    }
    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        /*builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Web API",
                Description = "An ASP.NET Core Web API",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Example Contact",
                    Url = new Uri("https://example.com/contact")
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });*/
        
        builder.Services.AddSwaggerGen(op =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            op.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        builder.Services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver"));
        });
        
        builder.Services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        
    }
    private static void ConfigureCors(WebApplicationBuilder builder)
    {
        //Cors
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "mycors",
                policy  =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }
    private static void ConfigureServicesLayers(WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        Application.ConfigureServices.AddServices(builder.Services, builder.Configuration, assembly);
        Infraestructure.ConfigureServices.AddServices(builder.Services, builder.Configuration);
    }
    private static void AddAttributesAndFilters(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ValidationFilterAttribute>();
    }
}
