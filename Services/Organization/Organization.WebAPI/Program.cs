using Consul;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Organization.Application;
using Organization.Application.Common.Middlewares;
using Organization.Application.Common.Services;
using Organization.Application.IntegrationEvent.Handlers;
using Organization.Infrastructure;
using Organization.Infrastructure.Telegram;
using Organization.WebAPI.Extensions;
using Shared.Events;
using Shared.Middlewares;
using System.Text;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
//var env = "Production";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.builder.Services.AddControllers(options =>
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddHttpClient();

builder.Services.Configure<TelegramConfiguration>(
    builder.Configuration.GetSection("TelegramConfiguration")
);

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureAuth(builder.Configuration);

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
_eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
_eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, OrderStockConfirmedIntegrationEventHandler>();

app.RegisterConsulService(builder.Configuration, app.Lifetime);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RestrictAccessMiddleware>();
app.UseMiddleware<TokenCheckerMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();