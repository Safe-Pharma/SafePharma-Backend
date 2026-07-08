using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class SupplierPaymentMapper
    {
        public static SupplierPaymentDto ToDto(this SupplierPayment entity)
        {
            return new SupplierPaymentDto
            {
                Id = entity.Id,
                SupplierId = entity.SupplierId,
                SupplierName = entity.Supplier?.Name ?? string.Empty,
                Amount = entity.Amount,
                PaymentMethod = entity.PaymentMethod,
                Note = entity.Note,
                PaidAt = entity.PaidAt,
            };
        }
    }
}
