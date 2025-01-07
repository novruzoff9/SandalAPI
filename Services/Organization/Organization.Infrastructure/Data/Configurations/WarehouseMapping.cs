using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class WarehouseMapping : BaseEntityMapping<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.CompanyID).HasMaxLength(36).IsRequired();


        builder.HasOne(e => e.Company)
            .WithMany(e => e.Warehouses)
            .HasForeignKey(e => e.CompanyID);
    }
}


