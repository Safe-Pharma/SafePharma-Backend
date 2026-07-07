namespace SafePharma.BLL
{
    public class PaymentMethodReadDto
    {
        public Guid Id { get; set; }
        public string MethodName { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public List<PaymentMethodFieldDto> Fields { get; set; } = new();
    }
}