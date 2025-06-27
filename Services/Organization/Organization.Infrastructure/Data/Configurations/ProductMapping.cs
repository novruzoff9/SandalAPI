using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;
using Shared.Services;

namespace Organization.Infrastructure.Data.Configurations;

public class ProductMapping : BaseEntityMapping<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.HasIndex(e => e.CompanyId);

        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(256);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CompanyId);
    }
}

