using System.Net;
using Application.Common.Exceptions;
using Application.Common.Wrappers;
using Newtonsoft.Json;
using WebApi.Middlewares.Models;

namespace WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var message = "Internal Server error";
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        if (exception.GetType() == typeof(NotFoundException))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            message = exception.Message;
        }
        else if (exception.GetType() == typeof(DuplicateKeyException))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            message = exception.Message;
        }
        //await context.Response.WriteAsJsonAsync(ResponseService<dynamic>.Error(message));
        await context.Response.WriteAsync(JsonConvert.SerializeObject(ResponseService<dynamic>.Error(message)));
    }
}