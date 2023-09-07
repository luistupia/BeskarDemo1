using Application.Common.Wrappers;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi.Extensions;

public static class ValidationExtension
{
    public static ContentResult ToErrorsResult(this ValidationResult obj)
    {
        var content = new ContentResult
        {
            Content = JsonConvert.SerializeObject(ResponseService<string>.Error(string.Join("/n", obj.Errors))),
            StatusCode = StatusCodes.Status400BadRequest,
            ContentType = "Application/json"
        };
        return content;
    }
}