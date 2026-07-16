using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SafePharma.DAL
{
    public class ChronicConditionConfiguration : IEntityTypeConfiguration<ChronicCondition>
    {
        public void Configure(EntityTypeBuilder<ChronicCondition> builder)
        {
            builder.ToTable("ChronicConditions");

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
