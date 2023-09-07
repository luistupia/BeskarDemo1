using Application.Common.Wrappers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var obj = context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) as T;

        if (obj is null)
        {
            return Results.BadRequest();
        }

        var validationResult = await _validator.ValidateAsync(obj);

        if (validationResult.IsValid) return await next(context);
        
        //return Results.BadRequest(string.Join("/n", validationResult.Errors));
        var content = new ContentResult
        {
            Content = JsonConvert.SerializeObject(ResponseService<string>.Error(string.Join("/n", validationResult.Errors))),
            StatusCode = StatusCodes.Status400BadRequest,
            ContentType = "Application/json"
        };

        Results.BadRequest(content);

        return await next(context);
    }

}