using DAL.Entities;

namespace Application.Interfaces;

public interface IPostgresCacheService
{
    Task SetData(CacheItem cacheItem);
}