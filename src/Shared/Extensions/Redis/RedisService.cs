using StackExchange.Redis;

namespace Shared.Extensions.Redis;

public class RedisService
{
    private ConnectionMultiplexer _connectionMultiplexer;
    private readonly RedisConfiguration _redisConfiguration;

    public RedisService(RedisConfiguration redisConfiguration)
    {
        _redisConfiguration = redisConfiguration;
    }

    public void Connect()
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { $"{_redisConfiguration.Host}:{_redisConfiguration.Port}" },
            //Password = _redisConfiguration.Password,
            //Ssl = _redisConfiguration.Ssl,
            //AbortOnConnectFail = false
        };
        _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
    }

    public IServer GetServer()
    {
        var endpoint = _connectionMultiplexer.GetEndPoints().First();
        return _connectionMultiplexer.GetServer(endpoint);
    }

    public IDatabase GetDb(int db = 1) => _connectionMultiplexer.GetDatabase(db);
}
