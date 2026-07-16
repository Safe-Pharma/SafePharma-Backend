using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafePharma.DAL;

public class ManufacturerBarcodeConfiguration : IEntityTypeConfiguration<ManufacturerBarcode>
{
    public void Configure(EntityTypeBuilder<ManufacturerBarcode> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Barcode)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.Barcode)
               .IsUnique();

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Medicine)
               .WithMany(x => x.ManufacturerBarcodes)
               .HasForeignKey(x => x.MedicineId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}