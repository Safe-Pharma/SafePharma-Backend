namespace SafePharma.DAL
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } // ISO 2-letter, e.g. "AE"

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}