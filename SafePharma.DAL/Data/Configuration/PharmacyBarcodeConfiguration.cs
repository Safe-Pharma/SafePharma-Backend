using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafePharma.DAL;

public class PharmacyBarcodeConfiguration : IEntityTypeConfiguration<PharmacyBarcode>
{
    public void Configure(EntityTypeBuilder<PharmacyBarcode> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Barcode)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => new { x.PharmacyMedicineId, x.Barcode })
               .IsUnique();

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.PharmacyMedicine)
               .WithMany(x => x.PharmacyBarcodes)
               .HasForeignKey(x => x.PharmacyMedicineId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}