using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class ShelfMapping : BaseEntityMapping<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.Code).HasMaxLength(256).IsRequired();
        builder.Property(e => e.WarehouseID).HasMaxLength(36).IsRequired();

        builder.HasOne(e => e.Warehouse)
            .WithMany(e => e.Shelves)
            .HasForeignKey(e => e.WarehouseID);
    }
}


