namespace Order.Infrastructure.Redis;

public class RedisConfiguration
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string InstanceName { get; set; } = null!;
}