using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Infraestructure.Common.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IDistributedCache _memoryCache;
    private DistributedCacheEntryOptions _cacheOptions;

    public MemoryCacheService(IDistributedCache memoryCache, IOptions<CacheConfiguration> cacheConfig)
    {
        _memoryCache = memoryCache;
        var config = cacheConfig.Value; //Usando el patron IOptions para que la configuración sea singleton

        _cacheOptions = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(config.SlidingExpirationInMinutes))
            .SetAbsoluteExpiration(TimeSpan.FromHours(config.AbsoluteExpirationInHours));
    }

    public T? GetData<T>(string key)
    {
        var val = _memoryCache.Get(key);
        if (val is null) return default;
        var value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
        return value;
    }

    public bool TryGet<T>(string key, out T? value)
    {
        var val = _memoryCache.Get(key);
        value = default;
        if (val == null) return false;
        value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
        return true;
    }

    public Task SetAsync<T>(string key, T value)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
        return _memoryCache.SetAsync(key, bytes, _cacheOptions);
    } 

    public void Remove(string key) => _memoryCache.Remove(key);


    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}