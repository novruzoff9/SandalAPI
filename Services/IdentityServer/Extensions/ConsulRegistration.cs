using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace IdentityServer.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(cfg =>
            {
                var address = configuration["ConsulConfig:Address"];
                cfg.Address = new Uri(address);
                //cfg.Datacenter = configuration["ConsulConfig:Datacenter"];
            }));
            return services;
        }

        public static IApplicationBuilder RegisterConsulService(this IApplicationBuilder app, IConfiguration configuration, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var addess = addresses.Addresses.FirstOrDefault();
            var uri = new Uri(addess);

            var serviceName = configuration["ConsulConfig:ServiceName"];
            var serviceId = $"{serviceName}_{Environment.MachineName}";
            var serviceAddress = configuration["ConsulConfig:ServiceAddress"];
            var servicePort = int.Parse(configuration["ConsulConfig:ServicePort"]);
            var registration = new AgentServiceRegistration()
            {
                ID = serviceId,
                Name = serviceName,
                //Address = serviceAddress,
                Address = $"{uri.Host}",
                Port = servicePort,
                Tags = new[] { "identity", "identity service" }
            };
            logger.LogInformation("Registering service {ServiceName} with ID {ServiceId} at {ServiceAddress}:{ServicePort}", serviceName, serviceId, serviceAddress, servicePort);
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering service {ServiceName} with ID {ServiceId}", serviceName, serviceId);
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}