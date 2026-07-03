using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser ,ApplicationRole ,Guid>
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
        }
        public override int SaveChanges()
        {
            AuditLog();
            return base.SaveChanges();
        }


        private void AuditLog()
        {
            var dateTime = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = dateTime;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = dateTime;
                }
            }
        }
        public DbSet<Audit> Audit => Set<Audit>();
        public DbSet<PharmacySettings> PharmacySettings => Set<PharmacySettings>();
        public DbSet<Tax> Taxes => Set<Tax>();

    }
}
