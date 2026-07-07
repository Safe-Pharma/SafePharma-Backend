namespace SafePharma.DAL
{
    public static class PurchaseOrderItemSeeding
    {
        public static List<PurchaseOrderItem> GetPurchaseOrderItems()
        {
            return new List<PurchaseOrderItem>
            {
                // ===========================
                // PO-1001
                // ===========================

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000001"),
                    PurchaseOrderId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    MedicineId = Guid.Parse("50000000-0000-0000-0000-000000000001"), // Panadol
                    QuantityOrdered = 300,
                    UnitPrice = 10.50m
                },

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000002"),
                    PurchaseOrderId = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                    MedicineId = Guid.Parse("50000000-0000-0000-0000-000000000005"), // Vitamin C
                    QuantityOrdered = 20,
                    UnitPrice = 195m
                },

                // ===========================
                // PO-1002
                // ===========================

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000003"),
                    PurchaseOrderId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    MedicineId = Guid.Parse("50000000-0000-0000-0000-000000000002"), // Augmentin
                    QuantityOrdered = 80,
                    UnitPrice = 33m
                },

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000004"),
                    PurchaseOrderId = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                    MedicineId = Guid.Parse("50000000-0000-0000-0000-000000000003"), // Lantus
                    QuantityOrdered = 16,
                    UnitPrice = 125m
                },

                // ===========================
                // PO-1003
                // ===========================

                new PurchaseOrderItem
                {
                    Id = Guid.Parse("71000000-0000-0000-0000-000000000005"),
                    PurchaseOrderId = Guid.Parse("70000000-0000-0000-0000-000000000003"),
                    MedicineId = Guid.Parse("50000000-0000-0000-0000-000000000004"), // Nootropil
                    QuantityOrdered = 28,
                    UnitPrice = 60m
                }
            };
        }
    }
}