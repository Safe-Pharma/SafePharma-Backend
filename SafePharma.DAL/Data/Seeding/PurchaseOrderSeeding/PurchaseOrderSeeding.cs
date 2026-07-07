namespace SafePharma.DAL
{
    public static class PurchaseOrderSeeding
    {
        public static readonly Guid Po1001Id = Guid.Parse("70000000-0000-0000-0000-000000000001");
        public static readonly Guid Po1002Id = Guid.Parse("70000000-0000-0000-0000-000000000002");
        public static readonly Guid Po1003Id = Guid.Parse("70000000-0000-0000-0000-000000000003");

        public static List<PurchaseOrder> GetPurchaseOrders(List<Supplier> suppliers)
        {
            var mediRx = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var alShifa = Guid.Parse("30000000-0000-0000-0000-000000000002");
            var nourAlHayat = Guid.Parse("30000000-0000-0000-0000-000000000003");

            Guid SupplierIdFor(Guid pharmacyId, string supplierName) =>
                suppliers.First(s => s.PharmacyId == pharmacyId && s.Name == supplierName).Id;

            return new List<PurchaseOrder>
            {
                new PurchaseOrder
                {
                    Id = Po1001Id,
                    PharmacyId = mediRx,
                    OrderNumber = "PO-1001",
                    SupplierId = SupplierIdFor(mediRx, "MedSupply Co."),
                    OrderDate = new DateTime(2026, 6, 22),
                    ExpectedDate = new DateTime(2026, 6, 25),
                    Status = "Open",
                    TotalAmount = 7050m
                },
                new PurchaseOrder
                {
                    Id = Po1002Id,
                    PharmacyId = alShifa,
                    OrderNumber = "PO-1002",
                    SupplierId = SupplierIdFor(alShifa, "GulfPharma"),
                    OrderDate = new DateTime(2026, 6, 30),
                    ExpectedDate = new DateTime(2026, 7, 3),
                    Status = "Partially Received",
                    TotalAmount = 4640m
                },
                new PurchaseOrder
                {
                    Id = Po1003Id,
                    PharmacyId = nourAlHayat,
                    OrderNumber = "PO-1003",
                    SupplierId = SupplierIdFor(nourAlHayat, "MedSupply Co."),
                    OrderDate = new DateTime(2026, 7, 4),
                    ExpectedDate = new DateTime(2026, 7, 8),
                    Status = "Received",
                    TotalAmount = 1680m
                }
            };
        }
    }
}