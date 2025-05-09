using EventBus.Base.Abstraction;
using EventBus.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Order.Application;
using Order.Infrastructure;
using EventBus.Factory;
using Order.Application.IntegratonEvents.Handlers;
using Shared.Events.Events;
using Shared.Middlewares;
using Order.WebAPI.Extensions;
using System;

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

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("roles");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Services:IdentityService"];
        options.Audience = "OrganizationAPIFullAccess";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "sub",
            RoleClaimType = "roles",
            ValidateAudience = true
        };
    });

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