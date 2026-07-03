using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Tax>(entity =>
            {
                entity.Property(t => t.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(t => t.Rate)
                    .HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(s => s.PlanTier).HasMaxLength(20);
                entity.Property(s => s.BillingCycle).HasMaxLength(10);

                entity.HasOne(s => s.Pharmacy)
                      .WithOne(p => p.Subscription)
                      .HasForeignKey<Pharmacy>(p => p.SubscriptionId);
            });

            modelBuilder.Entity<PrimaryContact>(entity =>
            {
                entity.HasOne(pc => pc.Pharmacy)
                      .WithOne()
                      .HasForeignKey<PrimaryContact>(pc => pc.PharmacyId);

                entity.HasIndex(pc => pc.Email).IsUnique();
            });
        }
        public DbSet<Audit> Audit => Set<Audit>();
        public DbSet<PharmacySettings> PharmacySettings => Set<PharmacySettings>();
        public DbSet<Tax> Taxes => Set<Tax>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
        public DbSet<PrimaryContact> PrimaryContacts => Set<PrimaryContact>();

    }
}
