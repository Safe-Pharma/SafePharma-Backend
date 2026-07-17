using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class OrganImpairmentLevelConfiguration
    : IEntityTypeConfiguration<OrganImpairmentLevel>
    {
        public void Configure(EntityTypeBuilder<OrganImpairmentLevel> builder)
        {
            builder.ToTable("OrganImpairmentLevels");

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
