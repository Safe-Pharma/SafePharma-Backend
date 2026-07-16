using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class AllergyConfiguration : IEntityTypeConfiguration<Allergy>
    {
        public void Configure(EntityTypeBuilder<Allergy> builder)
        {
            builder.ToTable("Allergies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameEn)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.NameAr)
                .HasMaxLength(150)
                .IsRequired();
        }
    }
}
