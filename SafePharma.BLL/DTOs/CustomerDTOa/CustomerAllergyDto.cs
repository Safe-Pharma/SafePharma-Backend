namespace SafePharma.BLL
{
    public class CustomerAllergyDto
    {
        public Guid AllergyId { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
    }
}