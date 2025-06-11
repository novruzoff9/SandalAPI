using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class ShelfMapping : BaseEntityMapping<Shelf>
{
    public override void Configure(EntityTypeBuilder<Shelf> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.Code).HasMaxLength(256).IsRequired();

        builder.HasOne(e => e.Warehouse)
            .WithMany(e => e.Shelves)
            .HasForeignKey(e => e.WarehouseID)
            .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.NoAction);
    }
}


