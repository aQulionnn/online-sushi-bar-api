using System.Text.Json;

namespace DAL.Entities;

public record CacheItem(string Key, JsonElement Value);