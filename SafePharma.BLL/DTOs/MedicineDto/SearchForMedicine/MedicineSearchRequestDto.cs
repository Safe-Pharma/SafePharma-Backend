namespace SafePharma.BLL
{
    public class MedicineSearchRequestDto
    {
        public string Query { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
