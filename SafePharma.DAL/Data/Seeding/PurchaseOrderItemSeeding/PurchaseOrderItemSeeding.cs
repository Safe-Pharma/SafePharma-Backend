namespace SafePharma.DAL
{
    public static class PurchaseOrderItemSeeding
    {
        public static List<PurchaseOrderItem> GetPurchaseOrderItems(List<Medicine> medicines)
        {
            Guid MedicineIdFor(string tradeNameEn) =>
                medicines.First(m => m.TradeNameEn == tradeNameEn).Id;

            return new List<PurchaseOrderItem>
            {
                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000001"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1001Id,
                    MedicineId = MedicineIdFor("Panadol"),
                    QuantityOrdered = 300,
                    UnitPrice = 10.50m
                },
                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000002"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1001Id,
                    MedicineId = MedicineIdFor("Vitamin C Plus Collagen"),
                    QuantityOrdered = 20,
                    UnitPrice = 195m
                },
                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000003"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1002Id,
                    MedicineId = MedicineIdFor("Augmentin"),
                    QuantityOrdered = 80,
                    UnitPrice = 33m
                },
                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000004"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1002Id,
                    MedicineId = MedicineIdFor("Lantus Insulin"),
                    QuantityOrdered = 16,
                    UnitPrice = 125m
                },
                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000005"),
                    PurchaseOrderId = PurchaseOrderSeeding.Po1003Id,
                    MedicineId = MedicineIdFor("Nootropil"),
                    QuantityOrdered = 28,
                    UnitPrice = 60m
                }
            };
        }
    }
}