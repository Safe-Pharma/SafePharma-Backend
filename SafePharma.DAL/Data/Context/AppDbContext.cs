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

                entity.HasOne(t => t.Pharmacy)
                    .WithMany()
                    .HasForeignKey(t => t.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => new { t.PharmacyId, t.Name }).IsUnique();
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

            modelBuilder.Entity<Pharmacy>(entity =>
            {
                entity.HasIndex(p => p.BusinessEmail).IsUnique();

                entity.HasIndex(p => p.TaxNumber)
                      .IsUnique()
                      .HasFilter("[TaxNumber] IS NOT NULL");

                entity.HasIndex(p => p.CommercialRegistration)
                      .IsUnique()
                      .HasFilter("[CommercialRegistration] IS NOT NULL");
            });
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(c => c.Code).IsUnique();
                entity.HasIndex(c => c.Name).IsUnique();
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasOne(c => c.Country)
                      .WithMany(country => country.Cities)
                      .HasForeignKey(c => c.CountryId);

                // Same city name can exist in different countries, just not duplicated within one country
                entity.HasIndex(c => new { c.CountryId, c.Name }).IsUnique();
            });

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.Property(m => m.TradeNameAr)
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(m => m.TradeNameEn)
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(m => m.ScientificName)
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(m => m.Category)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(m => m.UnitOfSale)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(m => m.TherapeuticCategory)
                    .HasMaxLength(100);
                entity.Property(m => m.Manufacturer)
                    .HasMaxLength(255);
                entity.Property(m => m.CountryOfOrigin)
                    .HasMaxLength(100);
                entity.Property(m => m.StorageConditions)
                    .HasMaxLength(100);
                entity.Property(m => m.PurchasePrice)
                    .HasColumnType("decimal(12,2)");
                entity.Property(m => m.SellingPrice)
                    .HasColumnType("decimal(12,2)");

                entity.HasOne(m => m.Tax)
                    .WithMany()
                    .HasForeignKey(m => m.TaxId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.ScientificName);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Medicine_PurchasePrice",
                        "[PurchasePrice] >= 0");
                    t.HasCheckConstraint(
                        "CK_Medicine_SellingPrice",
                        "[SellingPrice] >= 0");
                });
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(s => s.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(s => s.Outstanding)
                    .HasColumnType("decimal(12,2)");

                entity.HasOne(s => s.Pharmacy)
                    .WithMany()
                    .HasForeignKey(s => s.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Country)
                    .WithMany()
                    .HasForeignKey(s => s.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => new { s.PharmacyId, s.Name }).IsUnique();
            });
<<<<<<< HEAD
            modelBuilder.Entity<PaymentVerification>(entity =>
            {
                entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(p => p.PaidAmount).HasColumnType("decimal(12,2)");
                entity.Property(p => p.PaymentMethod).HasMaxLength(50);
                entity.Property(p => p.TransactionReference).HasMaxLength(100);

                entity.HasOne(p => p.Subscription)
                      .WithMany(s => s.PaymentVerifications)
                      .HasForeignKey(p => p.SubscriptionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(s => s.PlanTier).HasMaxLength(20);
                entity.Property(s => s.BillingCycle).HasMaxLength(10);
                entity.Property(s => s.SequenceNumber).UseIdentityColumn();   // NEW — auto-incrementing, independent of the Guid Id

                entity.HasOne(s => s.Pharmacy)
                      .WithOne(p => p.Subscription)
                      .HasForeignKey<Pharmacy>(p => p.SubscriptionId);
            });

            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.Property(p => p.MonthlyPrice).HasColumnType("decimal(10,2)");
                entity.Property(p => p.YearlyPrice).HasColumnType("decimal(10,2)");
                entity.HasIndex(p => p.Tier).IsUnique();
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.Property(m => m.MethodName).HasMaxLength(50);
=======

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(po => po.TotalAmount)
                    .HasPrecision(18, 2);

                entity.HasOne(po => po.Pharmacy)
                    .WithMany(p => p.PurchaseOrders)
                    .HasForeignKey(po => po.PharmacyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(po => po.Supplier)
                    .WithMany()
                    .HasForeignKey(po => po.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.Property(i => i.UnitPrice)
                    .HasPrecision(18, 2);

                entity.HasOne(i => i.Medicine)
                    .WithMany()
                    .HasForeignKey(i => i.MedicineId)
                    .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(i => i.PurchaseOrder)
                    .WithMany(po => po.Items)
                    .HasForeignKey(i => i.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade); 
>>>>>>> main
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
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
        public DbSet<PrimaryContact> PrimaryContacts => Set<PrimaryContact>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
<<<<<<< HEAD
        public DbSet<PaymentVerification> PaymentVerifications => Set<PaymentVerification>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
=======
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrdersItems => Set<PurchaseOrderItem>();
>>>>>>> main

    }
}
