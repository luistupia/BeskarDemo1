using System.Reflection;
using Application.Common.Interfaces;
using Application.Common.Mapping;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(IServiceCollection services,
        IConfiguration configuration,Assembly? assembly = null)
    {

        //services.AddScoped<IValidator<CreateCustomerRequest>, CreateCustomerValidator>();
        //services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
        
        //Configuracion de Fluent Validations
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
 
        services.AddTransient<IMapper, MappingHelper>();
        services.AddTransient<ICustomerService, CustomerService>();
        
        return services;
    }
}