#!/bin/bash
set -e

# Kullanıcıdan proje adını al
echo "Lütfen proje adını girin:"
read projectName

# Ana klasör ve proje klasörlerini oluştur
mkdir -p $projectName
cd $projectName

# Class Library projelerini oluştur
dotnet new classlib -n "$projectName.Domain"
dotnet new classlib -n "$projectName.Infrastructure"
dotnet new classlib -n "$projectName.Application"

# Web API projesini oluştur
dotnet new webapi -n "$projectName.WebAPI" --no-https

# Domain iç yapısı
cd "$projectName.Domain"
mkdir -p Common Constants Entities Enums Events Exceptions ValueObjects

cat <<EOL > Common/BaseEntity.cs
namespace $projectName.Domain.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }
    }
}
EOL

cat <<EOL > Common/BaseAuditableEntity.cs
namespace $projectName.Domain.Common
{
    public class BaseAuditableEntity : BaseEntity
    {
        public DateTimeOffset Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
EOL

cd ..

# Infrastructure iç yapısı
cd "$projectName.Infrastructure"
mkdir -p Data/Configurations Data/Interceptors
touch DependencyInjection.cs GlobalUsings.cs

cat <<EOL > DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using $projectName.Application.Common.Interfaces;
using $projectName.Infrastructure.Data;

namespace $projectName.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        return services;
    }
}
EOL

cat <<EOL > Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using $projectName.Application.Common.Interfaces;
using System.Reflection;

namespace $projectName.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
EOL

cat <<EOL > Data/Configurations/BaseEntityMapping.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using $projectName.Domain.Common;

namespace $projectName.Infrastructure.Data.Configurations
{
    public class BaseEntityMapping<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
EOL

cd ..

# Application iç yapısı
cd "$projectName.Application"
mkdir -p Common/Behaviors Common/Exceptions Common/Interfaces Common/Mapping Common/Models
touch DependencyInjection.cs GlobalUsings.cs

cat <<EOL > DependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace $projectName.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
EOL

cat <<EOL > GlobalUsings.cs
global using AutoMapper;
global using FluentValidation;
global using MediatR;
global using Ardalis.GuardClauses;
global using $projectName.Domain.Entities;
global using $projectName.Application.Common.Interfaces;
global using Microsoft.EntityFrameworkCore;
EOL

cat <<EOL > Common/Interfaces/IApplicationDbContext.cs
using System.Threading;
using System.Threading.Tasks;

namespace $projectName.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
EOL


cd ../..

# Nuget paketleri
dotnet add "$projectName/$projectName.Domain/$projectName.Domain.csproj" package Microsoft.EntityFrameworkCore

dotnet add "$projectName/$projectName.Infrastructure/$projectName.Infrastructure.csproj" package Microsoft.EntityFrameworkCore.SqlServer
dotnet add "$projectName/$projectName.Infrastructure/$projectName.Infrastructure.csproj" package Microsoft.EntityFrameworkCore

dotnet add "$projectName/$projectName.Application/$projectName.Application.csproj" package Microsoft.EntityFrameworkCore
dotnet add "$projectName/$projectName.Application/$projectName.Application.csproj" package FluentValidation.DependencyInjectionExtensions
dotnet add "$projectName/$projectName.Application/$projectName.Application.csproj" package Ardalis.GuardClauses
dotnet add "$projectName/$projectName.Application/$projectName.Application.csproj" package MediatR
dotnet add "$projectName/$projectName.Application/$projectName.Application.csproj" package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add "$projectName/$projectName.Application/$projectName.Application.csproj" package MediatR.Extensions.Microsoft.DependencyInjection

dotnet add "$projectName/$projectName.WebAPI/$projectName.WebAPI.csproj" package Microsoft.EntityFrameworkCore
dotnet add "$projectName/$projectName.WebAPI/$projectName.WebAPI.csproj" package Microsoft.EntityFrameworkCore.Tools

echo "✅ NuGet paketleri başarıyla yüklendi."

cd "$projectName"

# Projeler arası bağımlılıkları ayarla (Onion Architecture)
dotnet add "$projectName.Application/$projectName.Application.csproj" reference "$projectName.Domain/$projectName.Domain.csproj"
dotnet add "$projectName.Infrastructure/$projectName.Infrastructure.csproj" reference "$projectName.Application/$projectName.Application.csproj"
dotnet add "$projectName.Infrastructure/$projectName.Infrastructure.csproj" reference "$projectName.Domain/$projectName.Domain.csproj"
dotnet add "$projectName.WebAPI/$projectName.WebAPI.csproj" reference "$projectName.Application/$projectName.Application.csproj"
dotnet add "$projectName.WebAPI/$projectName.WebAPI.csproj" reference "$projectName.Infrastructure/$projectName.Infrastructure.csproj"
dotnet add "$projectName.WebAPI/$projectName.WebAPI.csproj" reference "$projectName.Domain/$projectName.Domain.csproj"

echo "✅ Proje yapısı başarıyla oluşturuldu: $projectName"

cd ../..
# Solution dosyasına projeleri ekle (varsayım: MRPos.sln var)
dotnet sln Sandal.sln add Services/$projectName/$projectName.Domain/$projectName.Domain.csproj
dotnet sln Sandal.sln add Services/$projectName/$projectName.Infrastructure/$projectName.Infrastructure.csproj
dotnet sln Sandal.sln add Services/$projectName/$projectName.Application/$projectName.Application.csproj
dotnet sln Sandal.sln add Services/$projectName/$projectName.WebAPI/$projectName.WebAPI.csproj

echo "✅ Proje yapısı başarıyla oluşturuldu: $projectName"

# Sonda gözləmə
read -n 1 -s -r -p "Press any key to continue..."
echo
