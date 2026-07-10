using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafePharma.DAL.Data.Models;

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

                entity.HasIndex(m => m.ScientificName);
                entity.HasIndex(m => m.TradeNameEn).IsUnique();
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

            });
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

                    entity.HasOne(i => i.PharmacyMedicine)
                        .WithMany()
                        .HasForeignKey(i => i.PharmacyMedicineId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(i => i.PurchaseOrder)
                        .WithMany(po => po.Items)
                        .HasForeignKey(i => i.PurchaseOrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<PurchaseReceipt>(entity =>
                {
                    entity.Property(x => x.InvoiceTotal)
                          .HasPrecision(18, 2);

                    entity.Property(x => x.InvoiceNumber)
                          .HasMaxLength(100);

                    entity.HasOne(x => x.PurchaseOrder)
                          .WithMany()
                          .HasForeignKey(x => x.PurchaseOrderId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasMany(x => x.Items)
                          .WithOne(x => x.PurchaseReceipt)
                          .HasForeignKey(x => x.PurchaseReceiptId)
                          .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<PurchaseReceiptItem>(entity =>
                {
                    entity.Property(x => x.UnitPrice)
                        .HasPrecision(18, 2);

                    entity.Property(x => x.MedicineName)
                        .HasMaxLength(255);

                    entity.Property(x => x.BatchNumber)
                        .HasMaxLength(100);

                    entity.HasOne(x => x.PharmacyMedicine)
                        .WithMany()
                        .HasForeignKey(x => x.PharmacyMedicineId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(x => x.PurchaseOrderItem)
                        .WithMany()
                        .HasForeignKey(x => x.PurchaseOrderItemId)
                        .OnDelete(DeleteBehavior.Restrict);
                });


            modelBuilder.Entity<SupplierPayment>(entity =>
            {
                entity.Property(p => p.Amount)
                    .HasColumnType("decimal(12,2)");

                entity.Property(p => p.PaymentMethod)
                    .HasMaxLength(50);

                entity.HasOne(p => p.Supplier)
                    .WithMany()
                    .HasForeignKey(p => p.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.RecordedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.RecordedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PharmacyMedicine>(entity =>
            {
                entity.Property(mp => mp.PurchasePrice)
                    .HasColumnType("decimal(12,2)");
                entity.Property(mp => mp.SellingPrice)
                    .HasColumnType("decimal(12,2)");
                entity.Property(mp => mp.ChangedBy)
                    .HasMaxLength(255);

                entity.HasOne(mp => mp.Medicine)
                    .WithMany(m => m.PharmacyMedicines)
                    .HasForeignKey(mp => mp.MedicineId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mp => mp.Pharmacy)
                    .WithMany()
                    .HasForeignKey(mp => mp.PharmacyId)
                    .OnDelete(DeleteBehavior.Cascade);

                // One current price row per medicine per pharmacy
                entity.HasIndex(mp => new { mp.MedicineId, mp.PharmacyId }).IsUnique();

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_MedicinePrice_PurchasePrice",
                        "[PurchasePrice] >= 0");
                    t.HasCheckConstraint(
                        "CK_MedicinePrice_SellingPrice",
                        "[SellingPrice] >= 0");
                });
            });

            modelBuilder.Entity<PharmacyMedicineTax>(entity =>
            {
                entity.HasKey(x => new { x.PharmacyMedicineId, x.TaxId });

                entity.HasOne(x => x.PharmacyMedicine)
                    .WithMany(pm => pm.PharmacyMedicineTaxes)
                    .HasForeignKey(x => x.PharmacyMedicineId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Tax)
                    .WithMany()
                    .HasForeignKey(x => x.TaxId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ManufacturerBarcode>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Barcode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => x.Barcode)
                    .IsUnique();

                entity.HasOne(x => x.Medicine)
                    .WithMany(m => m.ManufacturerBarcodes)
                    .HasForeignKey(x => x.MedicineId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PharmacyBarcode>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Barcode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => new { x.PharmacyMedicineId, x.Barcode })
                    .IsUnique();

                entity.HasOne(x => x.PharmacyMedicine)
                    .WithMany(pm => pm.PharmacyBarcodes)
                    .HasForeignKey(x => x.PharmacyMedicineId)
                    .OnDelete(DeleteBehavior.Cascade);
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

        public DbSet<PaymentVerification> PaymentVerifications => Set<PaymentVerification>();
        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrdersItems => Set<PurchaseOrderItem>();
        public DbSet<Batch> Batches => Set<Batch>();


        public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();

        public DbSet<PharmacyMedicine> PharmacyMedicines => Set<PharmacyMedicine>();

        public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
        public DbSet<PurchaseReceiptItem> PurchaseReceiptItems { get; set; }

        public DbSet<ManufacturerBarcode> ManufacturerBarcodes => Set<ManufacturerBarcode>();
        public DbSet<PharmacyBarcode> PharmacyBarcodes => Set<PharmacyBarcode>();
        public DbSet<PharmacyMedicineTax> PharmacyMedicineTaxes => Set<PharmacyMedicineTax>();


    }
}
