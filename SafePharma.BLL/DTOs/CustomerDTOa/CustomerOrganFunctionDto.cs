namespace SafePharma.BLL
{
    public class CustomerOrganFunctionDto
    {
        public Guid Id { get; set; }
        public Guid OrganId { get; set; }
        public string OrganNameEn { get; set; } = string.Empty;
        public string OrganNameAr { get; set; } = string.Empty;
        public Guid OrganImpairmentLevelId { get; set; }
        public string ImpairmentLevelNameEn { get; set; } = string.Empty;
        public string ImpairmentLevelNameAr { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
    }
}