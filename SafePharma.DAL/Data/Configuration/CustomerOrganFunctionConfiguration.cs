using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class CustomerOrganFunctionConfiguration
    : IEntityTypeConfiguration<CustomerOrganFunction>
    {
        public void Configure(EntityTypeBuilder<CustomerOrganFunction> builder)
        {
            builder.ToTable("CustomerOrganFunctions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RecordedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerOrganFunctions)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Organ)
                .WithMany(x => x.CustomerOrganFunctions)
                .HasForeignKey(x => x.OrganId);

            builder.HasOne(x => x.OrganImpairmentLevel)
                .WithMany(x => x.CustomerOrganFunctions)
                .HasForeignKey(x => x.OrganImpairmentLevelId);

            builder.HasIndex(x => new
            {
                x.CustomerId,
                x.OrganId
            }).IsUnique();
        }
    }
}
