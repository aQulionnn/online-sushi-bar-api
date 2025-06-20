using System.Text.Json;
using Application.Interfaces;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PostgresCacheService(AppWriteDbContext writeDbContext) : ICacheService
{
    private readonly AppWriteDbContext _writeDbContext = writeDbContext;
    
    public async Task SetDataAsync<T>(string key, T data)
    {
        await _writeDbContext.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO cache(key, value)
            VALUES(@key, @value::jsonb)
            ON CONFLICT(key) DO UPDATE
            SET value = excluded.value;
            """,
            new { Key = key, Value = data.ToString() });
    }

    public async Task<T> GetDataAsync<T>(string key)
    {
        var data = await _writeDbContext.Database.SqlQuery<string>
            ($"SELECT value FROM cache WHERE key={key}").FirstOrDefaultAsync();
        if (string.IsNullOrEmpty(data))
            return default;
        
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task DeleteDataAsync(string key)
    {
        await _writeDbContext.Database.ExecuteSqlRawAsync("DELETE FROM cache WHERE key=@key");
    }
}