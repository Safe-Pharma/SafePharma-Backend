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

                Status = "Open",
                PaymentMethod = "Cash",

                Tax = 0,
                Discount = 0,
                Total = 0,
                AmountPaid = 0,

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
                Total = sale.Total,
                AmountPaid = sale.AmountPaid,
                RemainingAmount = 0,
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

            if (sale.Status != "Open")
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

            sale.Discount = sale.SaleItems.Sum(i => i.Discount);
            sale.Tax = sale.SaleItems.Sum(i => i.TaxAmount);
            sale.Total = sale.SaleItems.Sum(i => i.LineTotal);

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

            if (sale.Status != "Open")
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

            sale.Discount = sale.SaleItems.Sum(i => i.Discount);
            sale.Tax = sale.SaleItems.Sum(i => i.TaxAmount);
            sale.Total = sale.SaleItems.Sum(i => i.LineTotal);

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

            if (sale.Status != "Open")
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            var item = sale.SaleItems.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return GeneralResult<ReadSaleDto>.FailResult("Item not found in this sale");

            sale.SaleItems.Remove(item);

            sale.Discount = sale.SaleItems.Sum(i => i.Discount);
            sale.Tax = sale.SaleItems.Sum(i => i.TaxAmount);
            sale.Total = sale.SaleItems.Sum(i => i.LineTotal);

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
                Total = sale.Total,
                AmountPaid = sale.AmountPaid,
                RemainingAmount = sale.Total - sale.AmountPaid,
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
    }
}
