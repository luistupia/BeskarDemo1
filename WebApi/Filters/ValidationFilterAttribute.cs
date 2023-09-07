using Application.Common.Wrappers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace WebApi.Filters;

public class ValidationFilterAttribute : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilterAttribute(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await ValidateActionArguments(context);

        if (!context.ModelState.IsValid)
        {
            var messages = string.Join("; ", context.ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => !string.IsNullOrWhiteSpace(x.ErrorMessage) ? x.ErrorMessage : x.Exception!.Message.ToString()));

            var content = new ContentResult
            {
                Content = JsonConvert.SerializeObject(ResponseService<string>.Error($"Error Validation: {messages}")),
                StatusCode = StatusCodes.Status422UnprocessableEntity,
                ContentType = "Application/json"
            };

            context.Result = content;
            return;
        }

        await next();
    }
    
    private async Task ValidateActionArguments(ActionExecutingContext context)
    {
        foreach (var (_, value) in context.ActionArguments)
        {
            if (value is null)
                continue;

            await ValidateAsync(value, context.ModelState);
        }
    }
  
    private async Task ValidateAsync(object value, ModelStateDictionary modelState)
    {
        //Obtenemos el IValidator del objeto
        var validator = GetValidator(value.GetType());
        if (validator is null)
            return;

        //Creamos un ValidationContext que viene en FLuentValidation
        var context = new ValidationContext<object>(value);
        
        //Validamos el contexto u objeto para obtener los mensajes de error y el estado del ModelState
        var result = await validator.ValidateAsync(context);
        result.AddToModelState(modelState, string.Empty);
    }
    
    private IValidator? GetValidator(Type targetType)
    {
        //Crear un objeto tipo IValidator<CreateCustomerRequest> en tiempo de ejecución
        var validatorType = typeof(IValidator<>).MakeGenericType(targetType);
        
        //Se obtiene la instancia ya creada por Injección de dependencia
        var validator = (IValidator)_serviceProvider.GetService(validatorType)!;
        return validator;
    }
}