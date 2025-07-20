using IdentityServer.Protos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Organization.Application.Common.Interfaces;
using Organization.Application.Common.Services;
using Organization.Infrastructure.Data;
using Organization.Infrastructure.Services.gRPC;
using Organization.Infrastructure.Telegram;

namespace Organization.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("default");
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<ITelegramService, TelegramService>();

        services.AddScoped<IIdentityGrpcClient, IdentityGrpcClient>();

        services.AddGrpcClient<Identity.IdentityClient>(cfg =>
        {
            string identityGrpcService = configuration["Services:IdentityGrpcService"] ?? "http://localhost:5003";
            cfg.Address = new Uri(identityGrpcService);
        });


        return services;
    }
}