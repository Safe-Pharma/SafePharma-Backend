using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafePharma.DAL
{
    public class Audit
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public string oldValues { get; set; } = string.Empty;
        public string newValues { get; set; } = string.Empty;



    }
}
