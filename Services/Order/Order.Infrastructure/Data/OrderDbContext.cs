using Microsoft.EntityFrameworkCore;
using Order.Domain.Common;
using System.Reflection;
using Order.Domain.Entities;
using Order.Application.Common.Interfaces;

namespace Order.Infrastructure.Data
{
    public class OrderDbContext : DbContext, IOrderDbContext
    {
        public const string DEFAULT_SCHEMA = "order";
        public OrderDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Domain.Entities.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
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
