using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;
using Shared.Services;

namespace Organization.Infrastructure.Data.Configurations;

public class WarehouseMapping : BaseEntityMapping<Warehouse>
{
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);
        builder.HasIndex(e => e.CompanyID);

        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();


        builder.HasOne(e => e.Company)
            .WithMany(e => e.Warehouses)
            .HasForeignKey(e => e.CompanyID);
    }
}


