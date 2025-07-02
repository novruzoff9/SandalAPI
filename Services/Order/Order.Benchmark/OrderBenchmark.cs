using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application;
using Order.Application.Features.Orders.Queries.GetOrdersQuery;
using Order.Infrastructure;
using Shared.Services;
using System.Security.Claims;
using Shared.Extensions;
using Shared.Extensions.Redis;
using Order.Application.Features.Orders.Queries;

namespace Order.Benchmark;

[MemoryDiagnoser]
public class OrderBenchmark
{
    private IMediator _mediator = null!;

    [GlobalSetup]
    public void Setup()
    {
        // ✅ Config oxunur
        var currentDirectory = Directory.GetCurrentDirectory();
        var basePath = Path.GetFullPath(Path.Combine(currentDirectory, "../../../../../../.."));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // ✅ Layihə konfiqurasiya metodları
        services.AddInfrastructure(configuration);
        services.AddApplication(configuration);

        // ✅ Fake identity service
        services.AddScoped<ISharedIdentityService, FakeIdentityService>();

        services.AddConsul(configuration);
        services.AddRedis(configuration);

        // ✅ Fake user claim-ləri
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("sub", "test-user-id"),
            new Claim("companyId", "1e5dc749-5d62-43dd-8760-a9b5e2afe427"),
            new Claim(ClaimTypes.Role, "Boss")
        }, "TestAuth"));

        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor
        {
            HttpContext = context
        });

        var provider = services.BuildServiceProvider();

        _mediator = provider.GetRequiredService<IMediator>();
    }

    [Benchmark]
    public async Task GetOrdersQueryBenchmark()
    {
        await _mediator.Send(new GetOrdersQuery());
    }

    //[Benchmark]
    //public async Task GetOrdersQueryTestBenchmark()
    //{
    //    await _mediator.Send(new GetOrdersForBenchTestQuery());
    //}
}
