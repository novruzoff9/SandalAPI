using Microsoft.EntityFrameworkCore;
using Subscription.Application.Common.Interfaces;
using Subscription.Domain.Entities;
using System.Reflection;

namespace Subscription.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<CompanySubscription> CompanySubscriptions { get; set; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
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
