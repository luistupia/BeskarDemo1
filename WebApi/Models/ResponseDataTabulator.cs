using Newtonsoft.Json;
using Dapper;

namespace WebApi.Models;

public class ResponseDataTabulator<T>
{
    [JsonProperty("total")]
    public int Total { get; set; }
    
    [JsonProperty("total_pages")]
    public int TotalPages { get; set; }
    
    [JsonProperty("data")]
    public T? Data { get; set; }
}