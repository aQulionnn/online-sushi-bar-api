using Application.Interfaces;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PostgresCacheService(AppWriteDbContext writeDbContext) : IPostgresCacheService
{
    private readonly AppWriteDbContext _writeDbContext = writeDbContext;
    
    public async Task SetData(CacheItem cacheItem)
    {
        await _writeDbContext.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO cache(key, value)
            VALUES(@key, @value::jsonb)
            ON CONFLICT(key) DO UPDATE
            SET value = excluded.value;
            """,
            new { Key = cacheItem.Key, Value = cacheItem.Value.ToString() });
    }
}