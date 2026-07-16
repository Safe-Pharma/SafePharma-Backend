namespace SafePharma.DAL
{
    public class Allergy
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<CustomerAllergy> CustomerAllergies { get; set; } = new HashSet<CustomerAllergy>();
    }
}
