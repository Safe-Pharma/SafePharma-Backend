namespace SafePharma.DAL
{
    public static class PurchaseOrderSeeding
    {
        public static List<PurchaseOrder> GetPurchaseOrders()
        {
            return new List<PurchaseOrder>
            {
                new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = Guid.Parse("30000000-0000-0000-0000-000000000001"), // MediRx Pharmacy
                    OrderNumber = "PO-1001",
                    SupplierId = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    OrderDate = new DateTime(2026, 6, 22),
                    ExpectedDate = new DateTime(2026, 6, 25),
                    Status = "Open",
                    TotalAmount = 7050m
                },

                new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = Guid.Parse("30000000-0000-0000-0000-000000000002"), // Al Shifa Pharmacy
                    OrderNumber = "PO-1002",
                    SupplierId = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                    OrderDate = new DateTime(2026, 6, 30),
                    ExpectedDate = new DateTime(2026, 7, 3),
                    Status = "Partially Received",
                    TotalAmount = 4640m
                },

                new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = Guid.Parse("30000000-0000-0000-0000-000000000003"), // Nour Al Hayat
                    OrderNumber = "PO-1003",
                    SupplierId = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                    OrderDate = new DateTime(2026, 7, 4),
                    ExpectedDate = new DateTime(2026, 7, 8),
                    Status = "Received",
                    TotalAmount = 1680m
                }
            };
        }
    }
}