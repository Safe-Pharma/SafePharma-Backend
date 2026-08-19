namespace SafePharma.BLL
{
    /// <summary>
    /// One line of a local (not-yet-persisted) cart being sent to checkout.
    /// Mirrors CreateSaleItemsDto — no BatchId, it's still resolved FIFO
    /// (nearest-expiry) server-side, exactly like AddItemToSale does today.
    /// </summary>
    public record CheckoutItemDto(
        Guid PharmacyMedicineId,
        Guid? CustomerId,
        int Quantity,
        decimal Discount,
        decimal TaxAmount);

    /// <summary>
    /// The whole local cart, submitted once at the moment of payment.
    /// Nothing about the cart touches the database before this call —
    /// see SaleManager.Checkout.
    /// </summary>
    public record CheckoutDto(
        Guid? CustomerId,
        List<CheckoutItemDto> Items,
        decimal DiscountAmount,
        Guid? TaxId,
        decimal AmountPaidByCash,
        decimal AmountPaidByCard);

    /// <summary>
    /// Read-only stock/price preview for a single medicine, used by the POS
    /// while a cart is still purely local — never touches Sales/SaleItems.
    /// </summary>
    public class StockAvailabilityDto
    {
        public int AvailableQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
