using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class CustomerMedicineHistoryMapper
    {
        public static CustomerMedicineHistoryDto ToDto(this CustomerMedicineHistory entity)
        {
            var isGlobalMatch = entity.MedicineId is not null;

            return new CustomerMedicineHistoryDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                MedicineId = entity.MedicineId,
                IsGlobalMatch = isGlobalMatch,
                MedicineName = isGlobalMatch
                    ? entity.Medicine?.TradeNameEn ?? string.Empty
                    : entity.TradeName ?? string.Empty,
                ScientificName = isGlobalMatch
                    ? entity.Medicine?.ScientificName ?? string.Empty
                    : entity.ScientificName ?? string.Empty,
                PurchaseDate = entity.PurchaseDate,
                Quantity = entity.Quantity,
                IsActive = entity.IsActive,
                Notes = entity.Notes ?? string.Empty,
            };
        }

        public static CustomerMedicineHistory ToEntity(this CreateCustomerMedicineHistoryDto dto)
        {
            return new CustomerMedicineHistory
            {
                MedicineId = dto.MedicineId,
                TradeName = dto.TradeName,
                ScientificName = dto.ScientificName,
                PurchaseDate = dto.PurchaseDate ?? DateTime.UtcNow,
                Quantity = dto.Quantity,
                IsActive = dto.IsActive,
                Notes = dto.Notes,
            };
        }
    }
}