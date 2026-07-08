namespace SafePharma.DAL
{
    public static class SupplierPaymentSeeding
    {
        public static List<SupplierPayment> GetPayments()
        {
            
            var recordedByPerPharmacy = new Dictionary<int, Guid>
            {
                { 1, Guid.Parse("99999999-9999-9999-9999-999999999999") }, // admin
                { 2, Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }, // assistant
                { 3, Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd") }, // pharmassist
            };

            var payments = new List<SupplierPayment>();

            foreach (var (pharmacyIndex, recordedBy) in recordedByPerPharmacy)
            {
                var medSupplyId = SupplierSeeding.GetSeededSupplierId(pharmacyIndex, 1); 
                var gulfPharmaId = SupplierSeeding.GetSeededSupplierId(pharmacyIndex, 2); 

                payments.Add(new SupplierPayment
                {
                    Id = Guid.Parse($"6000000{pharmacyIndex}-0000-0000-0000-000000000001"),
                    SupplierId = medSupplyId,
                    RecordedBy = recordedBy,
                    Amount = 8000m,
                    PaymentMethod = SupplierPaymentMethods.BankTransfer,
                    Note = "TRX-8891",
                    PaidAt = new DateTime(2026, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2026, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                });

                payments.Add(new SupplierPayment
                {
                    Id = Guid.Parse($"6000000{pharmacyIndex}-0000-0000-0000-000000000002"),
                    SupplierId = gulfPharmaId,
                    RecordedBy = recordedBy,
                    Amount = 2500m,
                    PaymentMethod = SupplierPaymentMethods.Cheque,
                    Note = "CHQ-0451",
                    PaidAt = new DateTime(2026, 7, 3, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2026, 7, 3, 0, 0, 0, DateTimeKind.Utc),
                });
            }

            return payments;
        }
    }
}