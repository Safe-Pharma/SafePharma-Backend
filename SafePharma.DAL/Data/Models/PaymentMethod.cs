namespace SafePharma.DAL
{
    public class PaymentMethod : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string MethodName { get; set; }        // "Instapay" | "Bank Transfer" | "Vodafone Cash"
        public string FieldsJson { get; set; } = "[]"; // JSON list of {Label, Value} rows shown in the UI
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}