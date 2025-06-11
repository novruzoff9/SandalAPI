using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organization.Domain.Entities;

namespace Organization.Infrastructure.Data.Configurations;

public class ShelfProductMapping : BaseEntityMapping<ShelfProduct>
{
    public override void Configure(EntityTypeBuilder<ShelfProduct> builder)
    {
        base.Configure(builder);
        builder.Property(e => e.Quantity).IsRequired();

        builder
            .HasOne(sp => sp.Shelf)
            .WithMany(s => s.ShelfProducts)
            .HasForeignKey(sp => sp.ShelfID)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(sp => sp.Product)
            .WithMany(p => p.ShelfProducts)
            .HasForeignKey(sp => sp.ProductID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}


