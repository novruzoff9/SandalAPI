using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Common.Interfaces;
using Order.Infrastructure.Data;

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

        return services;
    }
}
