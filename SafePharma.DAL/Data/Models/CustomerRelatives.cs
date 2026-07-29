using System.ComponentModel.DataAnnotations.Schema;

namespace SafePharma.DAL
{
    public class CustomerRelative : IAuditableEntity
    {
        [key]
        public Guid Id { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("Relative")]
        public Guid RelativeId { get; set; }
        public Customer Relative { get; set; }
        public bool HasAccessToRelative { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
