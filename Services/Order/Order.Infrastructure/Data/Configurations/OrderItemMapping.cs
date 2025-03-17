using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Infrastructure.Data.Configurations;

public class OrderItemMapping : BaseEntityMapping<OrderItem>, IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("OrderItems", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasOne(e => e.Order)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.OrderId);
    }
}

