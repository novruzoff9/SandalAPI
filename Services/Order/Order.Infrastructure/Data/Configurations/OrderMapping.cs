using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure.Data.Configurations;

public class OrderMapping : BaseEntityMapping<Domain.Entities.Order>, IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        base.Configure(builder);

        builder.ToTable("Orders", OrderDbContext.DEFAULT_SCHEMA);

        builder.HasMany(e => e.Products)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId);

        builder.OwnsOne(e => e.Address);

        builder.Property<int>("_statusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("StatusId")
                .IsRequired();

        var navigation = builder.Metadata.FindNavigation(nameof(Domain.Entities.Order.Products));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey("_statusId");
    }
}
