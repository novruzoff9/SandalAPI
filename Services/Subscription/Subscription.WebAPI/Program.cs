using Microsoft.EntityFrameworkCore;
using Shared.Middlewares;
using Subscription.Application;
using Subscription.Infrastructure;
using Subscription.Infrastructure.Data;
using Shared.Extensions;
using Shared.Extensions.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddConsul(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();

app.RegisterConsulService(builder.Configuration, app.Lifetime);
app.UseMiddleware<RestrictAccessMiddleware>();
app.UseMiddleware<Shared.Middlewares.ExceptionHandlingMiddleware>();
//app.UseMiddleware<TokenCheckerMiddleware>();

app.MapControllers();

app.Run();
