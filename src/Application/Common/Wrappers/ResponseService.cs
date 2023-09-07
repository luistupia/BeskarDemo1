using Newtonsoft.Json;

namespace Application.Common.Wrappers;

public class ResponseService<TResponse>
{
    [JsonProperty("success")] public bool SuccessResult { get; }
    [JsonProperty("message")] public string? Message { get; set; }
    [JsonProperty("result")] public TResponse? Result { get; set; }
    
    public ResponseService(TResponse? response, string? message)
    {
        SuccessResult = true;
        Message = message;
        Result = response;
    }

    public ResponseService(string? message)
    {
        SuccessResult = false;
        Message = string.Empty;
        Message = message;
    }

    public ResponseService<TResponse> Success(Action<TResponse?> callback)
    {
        if (SuccessResult)
            callback(Result);
        return this;
    }

    public ResponseService<TResponse> Error(Action<string?> callback)
    {
        if (!SuccessResult)
            callback(Message);
        return this;
    }

    public static ResponseService<TResponse?> Ok(TResponse? response, string resultMensaje = "") 
        => new(response, resultMensaje);

    public static ResponseService<TResponse?> Error(string? error) => new(error);
}