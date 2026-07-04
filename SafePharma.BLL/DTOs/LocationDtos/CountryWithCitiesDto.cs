namespace SafePharma.BLL
{
    public class CountryWithCitiesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<CityDto> Cities { get; set; } = new();
    }
}