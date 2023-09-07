namespace Application.Common.Interfaces;

public interface ICacheService
{
    T? GetData<T>(string key);
    bool TryGet<T>(string cacheKey, out T? value);
    Task SetAsync<T>(string cacheKey, T value);
    void Remove(string cacheKey);
}