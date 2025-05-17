using EventBus.Base.Abstraction;
using Order.Application;
using Order.Application.IntegratonEvents.Handlers;
using Order.Infrastructure;
using Order.Infrastructure.Redis;
using Order.WebAPI.Extensions;
using Shared.Events;
using Shared.Extensions;
using Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);


var env = builder.Environment.EnvironmentName;
//var env = "Production";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("RedisConfiguration"));
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConsul(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

IEventBus _eventBus = app.Services.GetRequiredService<IEventBus>();
_eventBus.Subscribe<OrderStockNotEnoughIntegrationEvent, OrderStockNotEnoughIntegrationEventHandler>();
_eventBus.Subscribe<CustomerCreatedIntegrationEvent, CustomerCreatedIntegrationEventHandler>();
_eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, OrderStockConfirmedIntegrationEventHandler>();

app.RegisterConsulService(builder.Configuration, app.Lifetime);

app.UseAuthorization();

app.UseMiddleware<RestrictAccessMiddleware>();
app.UseMiddleware<TokenCheckerMiddleware>();

app.MapControllers();

app.Run();