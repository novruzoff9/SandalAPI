using Microsoft.Identity.Client;
using Order.Application.Common.Interfaces;
using Shared.ResultTypes;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Order.Infrastructure.Redis;


public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public RedisCacheService(RedisService redisService)
    {
        _database = redisService.GetDb();
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
    }
    public async Task<Response<bool>> DeleteAsync(string key)
    {
        bool result = await _database.KeyDeleteAsync(key);
        return result ? Response<bool>.Success(200) : Response<bool>.Fail("Failed to delete cache", 400);
    }

    public async Task<Response<T>> GetAsync<T>(string Id)
    {
        var jsonData = await _database.StringGetAsync(Id);
        if (jsonData.IsNullOrEmpty)
        {
            return Response<T>.Fail("Cache not found", 404);
        }
        return Response<T>.Success(JsonSerializer.Deserialize<T>(jsonData), 200);
    }

    public async Task<Response<bool>> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var jsonData = JsonSerializer.Serialize(value, _jsonSerializerOptions);
        bool result = await _database.StringSetAsync(key, jsonData, expiry);
        return result ? Response<bool>.Success(200) : Response<bool>.Fail("Failed to set cache", 400);
    }
}
