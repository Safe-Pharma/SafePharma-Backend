using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafePharma.DAL
{
    public class CustomerAllergyConfiguration : IEntityTypeConfiguration<CustomerAllergy>
    {
        public void Configure(EntityTypeBuilder<CustomerAllergy> builder)
        {
            builder.ToTable("CustomerAllergies");

            builder.HasKey(x => new
            {
                x.CustomerId,
                x.AllergyId
            });

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerAllergies)
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Allergy)
                .WithMany(x => x.CustomerAllergies)
                .HasForeignKey(x => x.AllergyId);
        }
    }
}
