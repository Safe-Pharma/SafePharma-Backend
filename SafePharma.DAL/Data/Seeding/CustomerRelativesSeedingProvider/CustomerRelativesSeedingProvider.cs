namespace SafePharma.DAL 
{
    public class CustomerRelativesSeedingProvider
    {

        public static List<CustomerRelative> GetCustomerRelatives(List<Customer> customers)
        {
            var seededAt = DateTime.UtcNow;

            var ahmed = customers[0];
            var sara = customers[1];
            var omar = customers[2];
            var hassan = customers[3];


            return new List<CustomerRelative>
    {
        // Ahmed <-> Sara
        new CustomerRelative
        {
            Id = Guid.NewGuid(),
            CustomerId = ahmed.Id,
            RelativeId = sara.Id,
            CreatedAt = seededAt,
            UpdatedAt = seededAt,
        },

       
        // Ahmed <-> Omar
        new CustomerRelative
        {
            Id = Guid.NewGuid(),
            CustomerId = ahmed.Id,
            RelativeId = omar.Id,
            CreatedAt = seededAt,
            UpdatedAt = seededAt,
        },

        

        // Sara <-> Omar
        new CustomerRelative
        {
            Id = Guid.NewGuid(),
            CustomerId = sara.Id,
            RelativeId = omar.Id,
            CreatedAt = seededAt,
            UpdatedAt = seededAt,
        },

        // hassan  <->  ahmed
         new CustomerRelative
        {
            Id = Guid.NewGuid(),
            CustomerId = hassan.Id,
            RelativeId = ahmed.Id,
            CreatedAt = seededAt,
            UpdatedAt = seededAt,
        },

         // hassan  <->  omar
         new CustomerRelative
        {
            Id = Guid.NewGuid(),
            CustomerId = hassan.Id,
            RelativeId = omar.Id,
            CreatedAt = seededAt,
            UpdatedAt = seededAt,
        },
         // hassan  <->  sara
         new CustomerRelative
        {
            Id = Guid.NewGuid(),
            CustomerId = sara.Id,
            RelativeId = hassan.Id,
            CreatedAt = seededAt,
            UpdatedAt = seededAt,
        },
            };
        }
    }
}
