using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class OrganConfiguration : IEntityTypeConfiguration<Organ>
    {
        public void Configure(EntityTypeBuilder<Organ> builder)
        {
            builder.ToTable("Organs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameEn)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.NameAr)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
