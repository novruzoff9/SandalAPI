using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class ShelfProductMapping : BaseEntityMapping<ShelfProduct>
{
    public void Configure(EntityTypeBuilder<ShelfProduct> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.ShelfID).HasMaxLength(36).IsRequired();
        builder.Property(e => e.ProductID).HasMaxLength(36).IsRequired();
        builder.Property(e => e.Quantity).IsRequired();

        builder.HasOne(e => e.Shelf)
            .WithMany(e => e.ShelfProducts)
            .HasForeignKey(e => e.ShelfID);

        builder.HasOne(e => e.Product)
            .WithMany(e => e.ShelfProducts)
            .HasForeignKey(e => e.ProductID);
    }
}


