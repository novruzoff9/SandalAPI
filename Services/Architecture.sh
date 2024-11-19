#!/bin/bash

# Kullanıcıdan proje adını al
echo "Lütfen proje adını girin:"
read projectName

# Proje klasörünü oluştur
mkdir $projectName
cd $projectName

# Class Library projelerini oluştur
dotnet new classlib -n "$projectName.Domain"
dotnet new classlib -n "$projectName.Infrastructure"
dotnet new classlib -n "$projectName.Application"

# Web API projesini controller'larla birlikte oluştur
dotnet new webapi --use-controllers -n "$projectName.WebAPI" --no-https

# Domain klasörü ve içindekiler
cd "$projectName.Domain"
mkdir -p Common Constants Entities Enums Events Exceptions ValueObjects

# BaseEntity ve BaseAuditableEntity sınıflarını oluştur
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

# Infrastructure klasörü ve içindekiler
cd "$projectName.Infrastructure"
mkdir -p Data/Configurations Data/Interceptors
touch DependencyInjection.cs GlobalUsings.cs

# ApplicationDbContext sınıfını oluştur ve DbContext'ten miras al
cat <<EOL > Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using $projectName.Domain.Common;
using $projectName.Application.Common.Interfaces;
using System.Reflection;

namespace $projectName.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<BaseEntity> BaseEntities { get; set; }
        public DbSet<BaseAuditableEntity> BaseAuditableEntities { get; set; }

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

# BaseEntityMapping sınıfını oluştur
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

# Application klasörü ve içindekiler
cd "$projectName.Application"
mkdir -p Common/Behaviors Common/Exceptions Common/Interfaces Common/Mapping Common/Models
touch DependencyInjection.cs GlobalUsings.cs

# IApplicationDbContext arayüzünü oluştur
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

cd ..

# Nuget paketlerini tüm Class Library-lere yükle
dotnet add "$projectName.Domain/$projectName.Domain.csproj" package Microsoft.EntityFrameworkCore

dotnet add "$projectName.Infrastructure/$projectName.Infrastructure.csproj" package Microsoft.EntityFrameworkCore.SqlServer
dotnet add "$projectName.Infrastructure/$projectName.Infrastructure.csproj" package Microsoft.EntityFrameworkCore

dotnet add "$projectName.Application/$projectName.Application.csproj" package Microsoft.EntityFrameworkCore
dotnet add "$projectName.Application/$projectName.Application.csproj" package FluentValidation.DependencyInjectionExtensions
dotnet add "$projectName.Application/$projectName.Application.csproj" package Ardalis.GuardClauses
dotnet add "$projectName.Application/$projectName.Application.csproj" package MediatR 

# Web API projesine nuget paketlerini yükle
dotnet add "$projectName.WebAPI/$projectName.WebAPI.csproj" package Microsoft.EntityFrameworkCore
dotnet add "$projectName.WebAPI/$projectName.WebAPI.csproj" package Microsoft.EntityFrameworkCore.Tools

# Proje yapısını tamamla
echo "Proje yapısı başarıyla oluşturuldu, IApplicationDbContext ve ApplicationDbContext eklendi, Entity Framework Core paketleri yüklendi."

# 
cd C:/Users/Novruzoff/source/repos/MRPos

dotnet sln MRPos.sln add  Services/$projectName/$projectName.Domain/$projectName.Domain.csproj
dotnet sln MRPos.sln add  Services/$projectName/$projectName.Infrastructure/$projectName.Infrastructure.csproj
dotnet sln MRPos.sln add  Services/$projectName/$projectName.Application/$projectName.Application.csproj
dotnet sln MRPos.sln add  Services/$projectName/$projectName.WebAPI/$projectName.WebAPI.csproj
