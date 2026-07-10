namespace SafePharma.BLL
{
    public class MedicineStatsDto
    {
        public int TotalMedicines { get; set; }
        public int Active { get; set; }
        public int Inactive { get; set; }
        public int PrescriptionRequired { get; set; }
        public int Controlled { get; set; }
        public int CategoriesCount { get; set; }
        public int BelowMinStock { get; set; }
    }
}