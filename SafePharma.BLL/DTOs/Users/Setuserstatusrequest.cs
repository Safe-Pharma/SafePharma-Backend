using System.ComponentModel.DataAnnotations;

namespace SafePharma.BLL
{
    public class SetuserStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
