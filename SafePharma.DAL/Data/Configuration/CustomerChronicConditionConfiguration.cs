using ecommerce.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class CustomerChronicConditionConfiguration : IEntityTypeConfiguration<CustomerChronicCondition>
    {
        public void Configure(EntityTypeBuilder<CustomerChronicCondition> builder)
        {
            builder.ToTable("CustomerChronicConditions");

            builder.HasKey(x => new
            {
                x.CustomerId,
                x.ChronicConditionId
            });

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerChronicConditions)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.ChronicCondition)
                .WithMany(x => x.CustomerChronicConditions)
                .HasForeignKey(x => x.ChronicConditionId);
        }
    }
}
