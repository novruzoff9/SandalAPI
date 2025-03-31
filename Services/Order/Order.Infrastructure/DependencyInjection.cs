using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Order.Application.Common.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Redis;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("default");
        services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        
        services.AddScoped<IOrderDbContext, OrderDbContext>();

        services.Configure<RedisConfiguration>(configuration.GetSection("RedisConfiguration"));

        services.AddSingleton<RedisService>(sp =>
        {
            var redisSettings = sp.GetRequiredService<IOptions<RedisConfiguration>>().Value;
            var redisService = new RedisService(redisSettings);
            redisService.Connect();
            return redisService;
        });

        services.AddScoped<IRedisCacheService, RedisCacheService>();

        return services;
    }
}
