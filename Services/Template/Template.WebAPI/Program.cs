using OfficeOpenXml;
using Template.WebAPI.DelegatingHandlers;
using Template.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string env = builder.Environment.EnvironmentName;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<TokenInjectionHandler>();
builder.Services.AddHttpContextAccessor();

// Configure EPPlus to use non-commercial license context
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddHttpClient("LocalClient", cfg =>
{
    cfg.BaseAddress = new Uri(builder.Configuration["Services:WebApiGateway:BaseUrl"] ?? "localhost:5000");
    cfg.DefaultRequestHeaders.Add("User-Agent", "Template.WebAPI");
    cfg.DefaultRequestHeaders.Add("Accept", "application/json");
})
    .AddHttpMessageHandler<TokenInjectionHandler>();

builder.Services.AddScoped<ITemplateGenerator, TemplateGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
