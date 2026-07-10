namespace SafePharma.DAL
{
    public static class PurchaseOrderItemSeeding
    {
        public static List<PurchaseOrderItem> GetPurchaseOrderItems(List<PharmacyMedicine> pharmacyMedicines)
        {
            Guid PharmacyMedicineIdFor(string tradeNameEn) =>
                pharmacyMedicines
                    .First(pm => pm.Medicine.TradeNameEn == tradeNameEn)
                    .Id;

            return new List<PurchaseOrderItem>
            {
                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000001"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1001Id,
                    PharmacyMedicineId = PharmacyMedicineIdFor("Panadol"),
                    QuantityOrdered = 300,
                    UnitPrice = 10.50m
                },

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000002"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1001Id,
                    PharmacyMedicineId = PharmacyMedicineIdFor("Vitamin C Plus Collagen"),
                    QuantityOrdered = 20,
                    UnitPrice = 195m
                },

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000003"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1002Id,
                    PharmacyMedicineId = PharmacyMedicineIdFor("Augmentin"),
                    QuantityOrdered = 80,
                    UnitPrice = 33m
                },

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000004"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1002Id,
                    PharmacyMedicineId = PharmacyMedicineIdFor("Lantus Insulin"),
                    QuantityOrdered = 16,
                    UnitPrice = 125m
                },

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000005"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1003Id,
                    PharmacyMedicineId = PharmacyMedicineIdFor("Nootropil"),
                    QuantityOrdered = 28,
                    UnitPrice = 60m
                }
            };
        }
    }
}