using Organization.Application.Common.Interfaces;
using System.Reflection;

namespace Organization.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<ShelfProduct> ShelfProducts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<StockHistory> StockHistories { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var changes = ChangeTracker.Entries<Product>()
                .Where(x => x.State == EntityState.Modified);

            foreach (var change in changes)
            {
                var originalValue = change.OriginalValues.GetValue<int>(nameof(Product.Quantity));
                var currentValue = change.CurrentValues.GetValue<int>(nameof(Product.Quantity));
                if (originalValue != currentValue)
                {
                    int difference = Math.Abs(currentValue - originalValue);
                    StockActionType type = currentValue > originalValue ?
                        StockActionType.Increase : StockActionType.Decrease;
                    var stockHistory = new 
                        StockHistory(change.Entity.CompanyId, change.Entity.Id, difference, type);
                    StockHistories.Add(stockHistory);
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}