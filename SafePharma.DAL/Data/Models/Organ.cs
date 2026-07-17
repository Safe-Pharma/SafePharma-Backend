namespace SafePharma.DAL
{
    public class Organ
    {
        public Guid Id { get; set; }

        public string NameEn { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public ICollection<CustomerOrganFunction> CustomerOrganFunctions { get; set; }
            = new HashSet<CustomerOrganFunction>();
    }
}
