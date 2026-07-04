

namespace SafePharma.BLL
{
    public class PrimaryContactDto
    {
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }   // hashed in the manager before save
    }
}
