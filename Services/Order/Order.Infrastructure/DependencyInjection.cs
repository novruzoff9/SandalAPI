using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Common.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Services.gRPC;
using OrderService.Protos.Identity;

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
        services.AddScoped<IIdentityGrpcClient, IdentityGrpcClient>();

        services.AddGrpcClient<Identity.IdentityClient>(cfg =>
        {
            string identityGrpcService = configuration["Services:IdentityGrpcService"] ?? "http://localhost:5003";
            cfg.Address = new Uri(identityGrpcService);
        });

        return services;
    }
}
