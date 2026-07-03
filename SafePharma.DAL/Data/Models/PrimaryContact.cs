namespace SafePharma.DAL
{
    public class PrimaryContact
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsApproved { get; set; }

        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; }
    }
}