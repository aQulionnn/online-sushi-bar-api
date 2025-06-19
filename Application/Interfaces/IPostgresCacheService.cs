using DAL.Entities;

namespace Application.Interfaces;

public interface IPostgresCacheService
{
    Task SetData(CacheItem cacheItem);
    Task<T> GetDataAsync<T>(string key);
    Task DeleteDataAsync(string key);
}