namespace SafePharma.BLL
{
    public class PaymentMethodUpsertDto
    {
        public string MethodName { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public List<PaymentMethodFieldDto> Fields { get; set; } = new();
    }
}