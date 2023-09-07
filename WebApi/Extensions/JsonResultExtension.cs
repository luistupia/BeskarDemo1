using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi.Extensions;

public static class JsonResultExtension
{
    public static ContentResult ToJsonResult(this object obj)
    {
        var content = new ContentResult
        {
            Content = JsonConvert.SerializeObject(obj),
            ContentType = "application/json",
            StatusCode = StatusCodes.Status200OK
        };
        return content;
    }

    public static ContentResult ToJsonNotFoundResult(this object obj)
    {
        var content = new ContentResult
        {
            Content = JsonConvert.SerializeObject(obj),
            ContentType = "application/json",
            StatusCode = StatusCodes.Status404NotFound
        };
        return content;
    }

    public static ContentResult ToJsonBadRequestResult(this object obj)
    {
        var content = new ContentResult
        {
            Content = JsonConvert.SerializeObject(obj),
            ContentType = "application/json",
            StatusCode = StatusCodes.Status400BadRequest
        };
        return content;
    }
}