namespace SafePharma.DAL
{
    public static class BatchSeeding
    {
        public static List<Batch> GetBatches()
        {
            var createdDate = new DateTime(2026, 3, 5, 10, 0, 0);

            var medicine1 = Guid.Parse("7e34bf5a-a20d-46cd-9f78-1290c5a736b5");
            var medicine2 = Guid.Parse("b2220e6b-d72d-4da8-9b2b-9848da69e408");

            var receipt1 = Guid.Parse("53b856d9-4023-4147-86c3-68495a7c0884");
            var receipt2 = Guid.Parse("ec9183a8-f8c6-4937-b875-0ac601062164");

            var pharmacy1 = Guid.Parse("30000000-0000-0000-0000-000000000001");


            return new List<Batch>
            {
                // Medicine 1
                new Batch
                {
                    Id = Guid.Parse("11111111-aaaa-1111-aaaa-111111111111"),
                    MedicineId = medicine1,
                    PurchaseReceiptItemId = receipt1,
                    BatchNumber = "1",
                    ExpiryDate = new DateTime(2026, 7, 20),
                    QuantityReceived = 100,
                    QuantityRemaining = 80,
                    PurchasePrice = 45.00m,
                    SellingPrice = 60.00m,
                    CreatedAt = createdDate,
                    PharmacyId=pharmacy1
                },
                new Batch
                {
                    Id = Guid.Parse("22222222-bbbb-2222-bbbb-222222222222"),
                    MedicineId = medicine1,
                    PurchaseReceiptItemId = receipt1,
                    BatchNumber = "2",
                    ExpiryDate = new DateTime(2029, 3, 31),
                    QuantityReceived = 150,
                    QuantityRemaining = 120,
                    PurchasePrice = 46.50m,
                    SellingPrice = 62.00m,
                    CreatedAt = createdDate.AddDays(1),
                    PharmacyId=pharmacy1

                },
                new Batch
                {
                    Id = Guid.Parse("33333333-cccc-3333-cccc-333333333333"),
                    MedicineId = medicine1,
                    PurchaseReceiptItemId = receipt1,
                    BatchNumber = "3",
                    ExpiryDate = new DateTime(2028, 8, 15),
                    QuantityReceived = 75,
                    QuantityRemaining = 20,
                    PurchasePrice = 44.00m,
                    SellingPrice = 59.00m,
                    CreatedAt = createdDate.AddDays(2),
                    PharmacyId=pharmacy1

                },

                // Medicine 2
                new Batch
                {
                    Id = Guid.Parse("44444444-dddd-4444-dddd-444444444444"),
                    MedicineId = medicine2,
                    PurchaseReceiptItemId = receipt2,
                    BatchNumber = "1",
                    ExpiryDate = new DateTime(2026, 7, 20),
                    QuantityReceived = 200,
                    QuantityRemaining = 170,
                    PurchasePrice = 18.00m,
                    SellingPrice = 27.50m,
                    CreatedAt = createdDate.AddDays(3),
                    PharmacyId=pharmacy1

                },
                new Batch
                {
                    Id = Guid.Parse("55555555-eeee-5555-eeee-555555555555"),
                    MedicineId = medicine2,
                    PurchaseReceiptItemId = receipt2,
                    BatchNumber = "2",
                    ExpiryDate = new DateTime(2028, 5, 31),
                    QuantityReceived = 120,
                    QuantityRemaining = 95,
                    PurchasePrice = 19.00m,
                    SellingPrice = 28.50m,
                    CreatedAt = createdDate.AddDays(4),
                    PharmacyId=pharmacy1

                },
                new Batch
                {
                    Id = Guid.Parse("66666666-ffff-6666-ffff-666666666666"),
                    MedicineId = medicine2,
                    PurchaseReceiptItemId = receipt2,
                    BatchNumber = "3",
                    ExpiryDate = new DateTime(2029, 1, 31),
                    QuantityReceived = 90,
                    QuantityRemaining = 45,
                    PurchasePrice = 17.50m,
                    SellingPrice = 26.00m,
                    CreatedAt = createdDate.AddDays(5),
                    PharmacyId=pharmacy1

                }
            };
        }
    }
}