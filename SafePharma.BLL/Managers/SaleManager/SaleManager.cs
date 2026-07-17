using SafePharma.Common;
using SafePharma.DAL;
using System.Threading.Tasks;

namespace SafePharma.BLL
{
    public class SaleManager : ISaleManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public SaleManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<ReadSaleDto>> CreateDraftSale(CreateDraftSaleDto dto, Guid pharmacyId, Guid userId)
        {
            var sale = new Sale
            {
                PharmacyId = pharmacyId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                ApplicationUserId = userId,

                Status = SaleStatus.Open,
                PaymentMethod = SalePaymentMethod.Cash,

                Tax = 0,
                Discount = 0,
                SubTotal = 0,
                GrandTotal = 0,
                AmountPaidByCard = 0,
                AmountPaidByCash = 0,
                AmountPaid = 0,
                Change = 0,

                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.SaleRepository.Add(sale);
            await _unitOfWork.SaveAsync();

            var result = new ReadSaleDto()
            {
                Id = sale.Id,
                PharmacyId = sale.PharmacyId,
                InvoiceNumber = sale.InvoiceNumber,
                ApplicationUserId = sale.ApplicationUserId,
                Status = sale.Status,
                PaymentMethod = sale.PaymentMethod,
                Tax = sale.Tax,
                Discount = sale.Discount,
                SubTotal = sale.SubTotal,
                GrandTotal = sale.GrandTotal,
                AmountPaid = sale.AmountPaid,
                AmountPaidByCash = sale.AmountPaidByCash,
                AmountPaidByCard = sale.AmountPaidByCard,
                Change = sale.Change,
                CreatedAt = sale.CreatedAt,
                Items = new List<ReadSaleItemsDto>()
            };

            return GeneralResult<ReadSaleDto>.SuccessResult(result);
        }

        public async Task<GeneralResult<ReadSaleDto>> AddItemToSale(
            Guid saleId,
            CreateSaleItemsDto dto,
            Guid pharmacyId,
            Guid userId)
        {
            if (userId == Guid.Empty)
                return GeneralResult<ReadSaleDto>.FailResult("Invalid user");

            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            if (dto.Quantity <= 0)
                return GeneralResult<ReadSaleDto>.FailResult("Quantity must be greater than zero");

            var batch = await _unitOfWork._batchRepository.GetById(dto.BatchId);

            if (batch == null || batch.MedicineId != dto.PharmacyMedicineId)
                return GeneralResult<ReadSaleDto>.FailResult("Batch not found or does not belong to this medicine");

            var existingItem = sale.SaleItems.FirstOrDefault(i =>
                i.PharmacyMedicineId == dto.PharmacyMedicineId &&
                i.BatchId == dto.BatchId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + dto.Quantity;

                if (newQuantity > batch.QuantityRemaining)
                    return GeneralResult<ReadSaleDto>.FailResult($"Only {batch.QuantityRemaining} units available");

                existingItem.Quantity = newQuantity;
                existingItem.Discount += dto.Discount;
                existingItem.TaxAmount += dto.TaxAmount;
                existingItem.LineTotal =
                    (existingItem.UnitPrice * existingItem.Quantity)
                    - existingItem.Discount
                    + existingItem.TaxAmount;
            }
            else
            {
                if (batch.QuantityRemaining < dto.Quantity)
                    return GeneralResult<ReadSaleDto>.FailResult($"Only {batch.QuantityRemaining} units available");

                var unitPrice = batch.SellingPrice;

                sale.SaleItems.Add(new SaleItem
                {
                    SaleId = sale.Id,
                    PharmacyMedicineId = dto.PharmacyMedicineId,
                    BatchId = dto.BatchId,
                    CustomerId = dto.CustomerId == Guid.Empty ? null : dto.CustomerId,
                    Quantity = dto.Quantity,
                    UnitPrice = unitPrice,
                    Discount = dto.Discount,
                    TaxAmount = dto.TaxAmount,
                    LineTotal = (unitPrice * dto.Quantity) - dto.Discount + dto.TaxAmount
                });
            }

            
            sale.SubTotal = sale.SaleItems.Sum(i => i.LineTotal);
            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;


            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = userId.ToString();

            await _unitOfWork.SaveAsync();

            var updatedSale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(updatedSale!));
        }
        public async Task<GeneralResult<ReadSaleDto>> UpdateSaleItem(Guid saleId, Guid itemId, UpdateSaleItemDto dto, Guid pharmacyId, Guid userId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            var item = sale.SaleItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return GeneralResult<ReadSaleDto>.FailResult("Item not found in this sale");

            if (dto.Quantity <= 0)
                return GeneralResult<ReadSaleDto>.FailResult("Quantity must be greater than zero");

            var batch = await _unitOfWork._batchRepository.GetById(item.BatchId);
            if (batch == null || batch.QuantityRemaining < dto.Quantity)
                return GeneralResult<ReadSaleDto>.FailResult($"Only {batch?.QuantityRemaining ?? 0} units available");

            item.Quantity = dto.Quantity;
            item.Discount = dto.Discount;
            item.TaxAmount = dto.TaxAmount;
            item.LineTotal = (item.UnitPrice * dto.Quantity) - dto.Discount + dto.TaxAmount;

           
            sale.SubTotal = sale.SaleItems.Sum(i => i.LineTotal);
            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;


            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = userId.ToString();
            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

        public async Task<GeneralResult<ReadSaleDto>> RemoveSaleItem(Guid saleId, Guid itemId, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            var item = sale.SaleItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return GeneralResult<ReadSaleDto>.FailResult("Item not found in this sale");

            sale.SaleItems.Remove(item);

           
            sale.SubTotal = sale.SaleItems.Sum(i => i.LineTotal);
            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;


            sale.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }
        private ReadSaleDto MapSaleToDto(Sale sale)
        {
            return new ReadSaleDto
            {
                Id = sale.Id,
                InvoiceNumber = sale.InvoiceNumber,
                PharmacyId = sale.PharmacyId,
                ApplicationUserId = sale.ApplicationUserId,
                CustomerId = sale.CustomerId,
                PaymentMethod = sale.PaymentMethod,
                Tax = sale.Tax,
                Discount = sale.Discount,
                SubTotal = sale.SubTotal,
                GrandTotal = sale.GrandTotal,
                AmountPaidByCard = sale.AmountPaidByCard,
                AmountPaidByCash = sale.AmountPaidByCash,
                AmountPaid = sale.AmountPaid,
                Change = sale.GrandTotal - sale.AmountPaid,
                Status = sale.Status,
                CreatedAt = sale.CreatedAt,
                Items = sale.SaleItems.Select(item => new ReadSaleItemsDto
                {
                    PharmacyMedicineId = item.PharmacyMedicineId,
                    MedicineName = item.PharmacyMedicine.TradeNameEn,
                    CustomerId = item.CustomerId,
                    CustomerName = item.Customer != null ? item.Customer.Name : string.Empty,
                    BatchId = item.BatchId,
                    BatchNumber = item.Batch.BatchNumber,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TaxAmount = item.TaxAmount,
                    LineTotal = item.LineTotal
                }).ToList()
            };
       }




        public async Task<GeneralResult<ReadSaleDto>> ApplyTax(Guid saleId, ApplySaleTaxDto dto, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            var tax = await _unitOfWork.TaxRepository.GetById(dto.TaxId);
            if (tax == null || tax.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Tax not found");

           // var subTotal = sale.SaleItems.Sum(i => i.UnitPrice * i.Quantity);
            var taxAmount = Math.Round(sale.SubTotal * (tax.Rate / 100m), 2);

            sale.Tax = taxAmount;
            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;
            sale.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

        public async Task<GeneralResult<ReadSaleDto>> ApplyDiscount(Guid saleId, ApplySaleDiscountDto dto, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            if (dto.DiscountAmount < 0)
                return GeneralResult<ReadSaleDto>.FailResult("Discount cannot be negative");

           // var subTotal = sale.SaleItems.Sum(i => i.UnitPrice * i.Quantity);

            if (dto.DiscountAmount > sale.SubTotal)
                return GeneralResult<ReadSaleDto>.FailResult("Discount cannot exceed the sale subtotal");

            sale.Discount = dto.DiscountAmount;
            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;
            sale.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

        public async Task<GeneralResult<ReadSaleDto>> Pay(Guid saleId, PaySaleDto dto, Guid pharmacyId, Guid userId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("This sale is not open for payment");

            if (sale.SaleItems.Count == 0)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot pay a sale with no items");

            if (dto.AmountPaidByCash < 0 || dto.AmountPaidByCard < 0)
                return GeneralResult<ReadSaleDto>.FailResult("Payment amounts cannot be negative");

            var totalPaid = dto.AmountPaidByCash + dto.AmountPaidByCard;

            if (totalPaid < sale.GrandTotal)
                return GeneralResult<ReadSaleDto>.FailResult(
                    $"Amount paid ({totalPaid}) is less than the sale total ({sale.GrandTotal}).");

            // re-check stock right before committing — protects against another sale
            // draining the same batch between "add item" time and "pay" time
            foreach (var item in sale.SaleItems)
            {
                var batch = await _unitOfWork._batchRepository.GetById(item.BatchId);
                if (batch == null || batch.QuantityRemaining < item.Quantity)
                {
                    return GeneralResult<ReadSaleDto>.FailResult(
                        $"Insufficient stock remaining for {item.PharmacyMedicine?.TradeNameEn ?? "one of the items"}.");
                }
            }

            foreach (var item in sale.SaleItems)
            {
                var batch = await _unitOfWork._batchRepository.GetById(item.BatchId);
                batch!.QuantityRemaining -= item.Quantity;
            }

            sale.AmountPaidByCash = dto.AmountPaidByCash;
            sale.AmountPaidByCard = dto.AmountPaidByCard;
            sale.AmountPaid = totalPaid;
            sale.PaymentMethod = dto.AmountPaidByCash > 0 && dto.AmountPaidByCard > 0
                ? SalePaymentMethod.Mixed
                : dto.AmountPaidByCard > 0
                    ? SalePaymentMethod.Card
                    : SalePaymentMethod.Cash;
            sale.Status = SaleStatus.Completed;
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = userId.ToString();

            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

        public async Task<GeneralResult<ReadSaleDto>> CancelSale(Guid saleId, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status == SaleStatus.Cancelled)
                return GeneralResult<ReadSaleDto>.FailResult("Sale is already cancelled");

            sale.Status = SaleStatus.Cancelled;
            sale.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

       
    }
}
