using EventBus.Base.Abstraction;
using OfficeOpenXml;
using Organization.Application;
using Organization.Application.Common.Middlewares;
using Organization.Application.IntegrationEvent.Handlers;
using Organization.Infrastructure;
using Organization.Infrastructure.Telegram;
using Shared.Events;
using Shared.Extensions;
using Shared.Extensions.Redis;
using Shared.Middlewares;

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

builder.Services.Configure<TelegramConfiguration>(options =>
    builder.Configuration.GetSection(nameof(TelegramConfiguration)).Bind(options)
);

builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddConsul(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure EPPlus to use non-commercial license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


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
_eventBus.Subscribe<CompanyAssignedPackIntegrationEvent, CompanyAssignedPackIntegrationEventHandler>();

app.RegisterConsulService(builder.Configuration, app.Lifetime);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<OrganizationDomainExceptionHandlingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RestrictAccessMiddleware>();
app.UseMiddleware<TokenCheckerMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();