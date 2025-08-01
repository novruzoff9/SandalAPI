using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Web.ApiGateway.Middlewares;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddEndpointsApiExplorer();

var env = builder.Environment.EnvironmentName;
builder.Configuration.AddJsonFile($"Configurations/ocelot.{env}.json");
builder.Services.AddOcelot().AddConsul();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServerUrl"];
        options.Audience = "GatewayAPIFullAccess";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "roles",
            NameClaimType = "sub",
            ValidateAudience = true
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseMiddleware<InterceptionMiddleware>();
//app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
