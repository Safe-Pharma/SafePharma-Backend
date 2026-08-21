using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TitleAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MessageAr)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.TitleEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MessageEn)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Priority)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ReferenceType)
                .HasConversion<int>();

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);


            builder.HasIndex(x => new
            {
                x.PharmacyId,
                x.Type,
                x.ReferenceId
            })
            .IsUnique();
        }
    }
}
