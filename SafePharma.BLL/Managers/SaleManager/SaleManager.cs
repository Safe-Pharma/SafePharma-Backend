using SafePharma.Common;
using SafePharma.DAL;
using System.Threading.Tasks;

namespace SafePharma.BLL
{
    public class SaleManager : ISaleManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationManager _notificationManager;
        public SaleManager(IUnitOfWork unitOfWork, INotificationManager notificationManager)
        {
            _unitOfWork = unitOfWork;
            _notificationManager = notificationManager;
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
                Status = sale.Status.ToString(),
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

            // Ownership check — without this, a caller could pass a PharmacyMedicineId
            // belonging to another pharmacy and sell its stock through this sale.
            var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
                .GetByIdAndPharmacy(dto.PharmacyMedicineId, pharmacyId);
            if (pharmacyMedicine == null)
                return GeneralResult<ReadSaleDto>.FailResult("Medicine not found in this pharmacy");

            var batch = await _unitOfWork._batchRepository.GetNearestExpiryBatchAsync(dto.PharmacyMedicineId);

            if (batch == null)
                return GeneralResult<ReadSaleDto>.FailResult("No available stock for this medicine");

            var existingItem = sale.SaleItems.FirstOrDefault(i =>
                i.PharmacyMedicineId == dto.PharmacyMedicineId &&
                i.BatchId == batch.Id);

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
                    BatchId = batch.Id,
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

            _unitOfWork.ClearTracking();
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

            item.CustomerId = dto.CustomerId == Guid.Empty ? null : dto.CustomerId;
            item.Quantity = dto.Quantity;
            item.Discount = dto.Discount;
            item.TaxAmount = dto.TaxAmount;
            item.LineTotal = (item.UnitPrice * dto.Quantity) - dto.Discount + dto.TaxAmount;


            sale.SubTotal = sale.SaleItems.Sum(i => i.LineTotal);
            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;


            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = userId.ToString();
            await _unitOfWork.SaveAsync();

            _unitOfWork.ClearTracking();
            var updatedSale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(updatedSale!));
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

        public async Task<GeneralResult<StockAvailabilityDto>> GetAvailability(Guid pharmacyMedicineId, Guid pharmacyId)
        {
            var pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
                .GetByIdAndPharmacy(pharmacyMedicineId, pharmacyId);
            if (pharmacyMedicine == null)
                return GeneralResult<StockAvailabilityDto>.NotFound("Medicine not found in this pharmacy");

            var availableQuantity = await _unitOfWork._batchRepository.GetAvailableQuantity(pharmacyMedicineId,pharmacyId);
            var batch = await _unitOfWork._batchRepository.GetNearestExpiryBatchAsync(pharmacyMedicineId);

            return GeneralResult<StockAvailabilityDto>.SuccessResult(new StockAvailabilityDto
            {
                AvailableQuantity = availableQuantity,
                UnitPrice = batch?.SellingPrice ?? 0
            });
        }

        /// <summary>
        /// Creates the Sale, adds every item, applies the sale-level discount/tax,
        /// and records payment — all in one atomic call, so a cart that existed
        /// only in the browser touches the database exactly once, at the moment
        /// of payment. Mirrors AddItemToSale (batch resolution/pricing) and Pay
        /// (stock deduction, customer medicine history, balance) internally.
        /// </summary>
        public async Task<GeneralResult<ReadSaleDto>> Checkout(CheckoutDto dto, Guid pharmacyId, Guid userId)
        {
            if (userId == Guid.Empty)
                return GeneralResult<ReadSaleDto>.FailResult("Invalid user");

            if (dto.Items == null || dto.Items.Count == 0)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot check out an empty cart");

            if (dto.AmountPaidByCash < 0 || dto.AmountPaidByCard < 0)
                return GeneralResult<ReadSaleDto>.FailResult("Payment amounts cannot be negative");

            var sale = new Sale
            {
                PharmacyId = pharmacyId,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                ApplicationUserId = userId,
                CustomerId = dto.CustomerId is null || dto.CustomerId == Guid.Empty ? null : dto.CustomerId,
                Status = SaleStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            // Cache lookups across lines (two lines can share the same medicine —
            // e.g. one item split between two family members).
            var pharmacyMedicineById = new Dictionary<Guid, PharmacyMedicine>();

            foreach (var line in dto.Items)
            {
                if (line.Quantity <= 0)
                    return GeneralResult<ReadSaleDto>.FailResult("Quantity must be greater than zero");

                if (!pharmacyMedicineById.TryGetValue(line.PharmacyMedicineId, out var pharmacyMedicine))
                {
                    pharmacyMedicine = await _unitOfWork.PharmacyMedicineRepository
                        .GetByIdAndPharmacy(line.PharmacyMedicineId, pharmacyId);
                    if (pharmacyMedicine == null)
                        return GeneralResult<ReadSaleDto>.FailResult("Medicine not found in this pharmacy");
                    pharmacyMedicineById[line.PharmacyMedicineId] = pharmacyMedicine;
                }

                var batch = await _unitOfWork._batchRepository.GetNearestExpiryBatchAsync(line.PharmacyMedicineId);
                if (batch == null)
                    return GeneralResult<ReadSaleDto>.FailResult($"No available stock for {pharmacyMedicine.TradeNameEn}.");

                var alreadyRequested = sale.SaleItems
                    .Where(i => i.PharmacyMedicineId == line.PharmacyMedicineId && i.BatchId == batch.Id)
                    .Sum(i => i.Quantity);

                if (alreadyRequested + line.Quantity > batch.QuantityRemaining)
                    return GeneralResult<ReadSaleDto>.FailResult(
                        $"Only {batch.QuantityRemaining} units available for {pharmacyMedicine.TradeNameEn}.");

                var unitPrice = batch.SellingPrice;
                sale.SaleItems.Add(new SaleItem
                {
                    SaleId = sale.Id,
                    PharmacyMedicineId = line.PharmacyMedicineId,
                    BatchId = batch.Id,
                    CustomerId = line.CustomerId is null || line.CustomerId == Guid.Empty ? null : line.CustomerId,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    Discount = line.Discount,
                    TaxAmount = line.TaxAmount,
                    LineTotal = (unitPrice * line.Quantity) - line.Discount + line.TaxAmount
                });
            }

            sale.SubTotal = sale.SaleItems.Sum(i => i.LineTotal);

            if (dto.DiscountAmount < 0)
                return GeneralResult<ReadSaleDto>.FailResult("Discount cannot be negative");
            if (dto.DiscountAmount > sale.SubTotal)
                return GeneralResult<ReadSaleDto>.FailResult("Discount cannot exceed the sale subtotal");
            sale.Discount = dto.DiscountAmount;

            if (dto.TaxId.HasValue)
            {
                var tax = await _unitOfWork.TaxRepository.GetById(dto.TaxId.Value);
                if (tax == null || tax.PharmacyId != pharmacyId)
                    return GeneralResult<ReadSaleDto>.FailResult("Tax not found");
                sale.Tax = Math.Round(sale.SubTotal * (tax.Rate / 100m), 2);
            }

            sale.GrandTotal = sale.SubTotal - sale.Discount + sale.Tax;

            var totalPaid = dto.AmountPaidByCash + dto.AmountPaidByCard;
            if (totalPaid < sale.GrandTotal)
                return GeneralResult<ReadSaleDto>.FailResult(
                    $"Amount paid ({totalPaid}) is less than the sale total ({sale.GrandTotal}).");

            // Deduct stock now that every validation above has passed — grouped
            // by batch since two lines can draw from the same one.
            foreach (var group in sale.SaleItems.GroupBy(i => i.BatchId))
            {
                var batch = await _unitOfWork._batchRepository.GetById(group.Key);
                batch!.QuantityRemaining -= group.Sum(i => i.Quantity);
            }

            // ---- same CustomerMedicineHistory upsert as Pay() ----
            if (sale.CustomerId.HasValue)
            {
                foreach (var item in sale.SaleItems)
                {
                    var pharmacyMedicine = pharmacyMedicineById[item.PharmacyMedicineId];
                    var medicineId = pharmacyMedicine.MedicineId;
                    var customerId = item.CustomerId ?? sale.CustomerId.Value;
                    var history = await _unitOfWork.CustomerMedicineHistoryRepository
                        .GetByCustomerAndMedicine(customerId, medicineId);

                    if (history != null)
                    {
                        history.Quantity += item.Quantity;
                        history.PurchaseDate = DateTime.UtcNow;
                        history.IsActive = true;
                        history.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        _unitOfWork.CustomerMedicineHistoryRepository.Add(new CustomerMedicineHistory
                        {
                            Id = Guid.NewGuid(),
                            CustomerId = customerId,
                            MedicineId = medicineId,
                            TradeName = medicineId is null ? pharmacyMedicine.TradeNameEn : null,
                            ScientificName = pharmacyMedicine.ScientificName,
                            PurchaseDate = DateTime.UtcNow,
                            Quantity = item.Quantity,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                        });
                    }
                }
            }

            sale.AmountPaidByCash = dto.AmountPaidByCash;
            sale.AmountPaidByCard = dto.AmountPaidByCard;
            sale.AmountPaid = totalPaid;
            sale.Change = totalPaid - sale.GrandTotal;
            sale.PaymentMethod = dto.AmountPaidByCash > 0 && dto.AmountPaidByCard > 0
                ? SalePaymentMethod.Mixed
                : dto.AmountPaidByCard > 0
                    ? SalePaymentMethod.Card
                    : SalePaymentMethod.Cash;
            sale.Status = SaleStatus.Completed;
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = userId.ToString();

            if (sale.CustomerId.HasValue)
            {
                var balance = await _unitOfWork.CustomerPharmacyBalanceRepository
                    .GetByCustomerAndPharmacy(sale.CustomerId.Value, pharmacyId);

                if (balance is not null)
                {
                    balance.TotalPaid += sale.AmountPaid;
                    balance.LastPaymentAt = DateTime.UtcNow;
                    balance.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    _unitOfWork.CustomerPharmacyBalanceRepository.Add(new CustomerPharmacyBalance
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = sale.CustomerId.Value,
                        PharmacyId = pharmacyId,
                        TotalPaid = sale.AmountPaid,
                        LastPaymentAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    });
                }
            }

            _unitOfWork.SaleRepository.Add(sale);
            await _unitOfWork.SaveAsync();

            //low stock notifications for all items in the sale, after committing the sale and deducting stock
            await CheckLowStockNotifications(
            sale.SaleItems.Select(i => i.PharmacyMedicineId),
            pharmacyId);
            //-----------------------------------------------

            _unitOfWork.ClearTracking();
            var savedSale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(sale.Id);

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(savedSale!));
        }
        private ReadSaleDto MapSaleToDto(Sale sale)
        {
            return new ReadSaleDto
            {
                Id = sale.Id,
                InvoiceNumber = sale.InvoiceNumber,
                PharmacyId = sale.PharmacyId,
                PharmacyName = sale.Pharmacy?.Name ?? string.Empty,
                ApplicationUserId = sale.ApplicationUserId,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer != null ? sale.Customer.Name : string.Empty,
                PaymentMethod = sale.PaymentMethod,
                Tax = sale.Tax,
                Discount = sale.Discount,
                SubTotal = sale.SubTotal,
                GrandTotal = sale.GrandTotal,
                AmountPaidByCard = sale.AmountPaidByCard,
                AmountPaidByCash = sale.AmountPaidByCash,
                AmountPaid = sale.AmountPaid,
                Change = sale.AmountPaid - sale.GrandTotal,
                Status = sale.Status.ToString(),
                CreatedAt = sale.CreatedAt,
                Items = sale.SaleItems.Select(item => new ReadSaleItemsDto
                {
                    Id = item.Id,
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

            // ============================================================
            // ---- new: upsert CustomerMedicineHistory per item ----
            // Only when the sale actually has a customer attached (walk-in
            // sales have CustomerId == null, so nothing to attach history to).
            // ============================================================
            if (sale.CustomerId.HasValue)
            {
                foreach (var item in sale.SaleItems)
                {
                    var medicineId = item.PharmacyMedicine.MedicineId;
                    var  customerId = item.CustomerId.HasValue ? item.CustomerId.Value : sale.CustomerId.Value;
                    var history = await _unitOfWork.CustomerMedicineHistoryRepository
                        .GetByCustomerAndMedicine(customerId, medicineId);

                    if (history != null)
                    {
                        // already taking/bought this medicine before — bump the quantity
                        // and refresh the purchase date instead of creating a duplicate row
                        history.Quantity += item.Quantity;
                        history.PurchaseDate = DateTime.UtcNow;
                        history.IsActive = true;
                        history.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        _unitOfWork.CustomerMedicineHistoryRepository.Add(new CustomerMedicineHistory
                        {
                            Id = Guid.NewGuid(),
                            CustomerId = customerId,
                            MedicineId = medicineId,
                            TradeName = medicineId is null ? item.PharmacyMedicine.TradeNameEn : null,
                            ScientificName = item.PharmacyMedicine.ScientificName, 
                            PurchaseDate = DateTime.UtcNow,
                            Quantity = item.Quantity,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                        });
                    }
                }
            }

            sale.AmountPaidByCash = dto.AmountPaidByCash;
            sale.AmountPaidByCard = dto.AmountPaidByCard;
            sale.AmountPaid = totalPaid;
            sale.Change = totalPaid - sale.GrandTotal;
            sale.PaymentMethod = dto.AmountPaidByCash > 0 && dto.AmountPaidByCard > 0
                ? SalePaymentMethod.Mixed
                : dto.AmountPaidByCard > 0
                    ? SalePaymentMethod.Card
                    : SalePaymentMethod.Cash;
            sale.Status = SaleStatus.Completed;
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedBy = userId.ToString();
            if (sale.CustomerId.HasValue)
            {
                var balance = await _unitOfWork.CustomerPharmacyBalanceRepository
                    .GetByCustomerAndPharmacy(sale.CustomerId.Value, pharmacyId);

                if (balance is not null)
                {
                    balance.TotalPaid += sale.AmountPaid;
                    balance.LastPaymentAt = DateTime.UtcNow;
                    balance.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    _unitOfWork.CustomerPharmacyBalanceRepository.Add(new CustomerPharmacyBalance
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = sale.CustomerId.Value,
                        PharmacyId = pharmacyId,
                        TotalPaid = sale.AmountPaid,
                        LastPaymentAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    });
                }
            }

            await _unitOfWork.SaveAsync();

            //low stock notifications for all items in the sale, after committing the sale and deducting stock
            await CheckLowStockNotifications(
                sale.SaleItems.Select(i => i.PharmacyMedicineId),
                pharmacyId);
            //-----------------------------------------------

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }


        public async Task<GeneralResult<ReadSaleDto>> CancelSale(Guid saleId, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult(
                    sale.Status == SaleStatus.Completed
                        ? "Cannot cancel a completed sale."
                        : "Sale is already cancelled");

            sale.Status = SaleStatus.Cancelled;
            sale.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

        /// <summary>
        /// Hard-deletes an untouched Open draft (e.g. an empty POS tab the pharmacist
        /// closed with the X button) — actually removes the row instead of just
        /// flipping its status, so closing tabs doesn't leave junk invoices behind.
        /// Only ever allowed for Open sales: a Completed sale is real transaction
        /// history and must never be deletable, and stock is only deducted at Pay
        /// time (see Pay above), so deleting an Open sale has no inventory to restore.
        /// </summary>
        public async Task<GeneralResult> DeleteDraftSale(Guid saleId, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult.NotFound("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult.FailResult(
                    "Only an open, unpaid draft can be deleted. Completed or cancelled sales are kept for the record.");

            _unitOfWork.SaleRepository.Delete(sale);
            await _unitOfWork.SaveAsync();

            return GeneralResult.SuccessResult("Draft sale deleted.");
        }

        public async Task<GeneralResult<ReadSaleDto>> SetCustomer(Guid saleId, SetSaleCustomerDto dto, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.FailResult("Sale not found");

            if (sale.Status != SaleStatus.Open)
                return GeneralResult<ReadSaleDto>.FailResult("Cannot modify a closed sale");

            sale.CustomerId = dto.CustomerId == Guid.Empty ? null : dto.CustomerId;
            sale.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            var updatedSale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);
            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(updatedSale!));
        }


        public async Task<GeneralResult<ReadSaleDto>> GetSaleById(Guid saleId, Guid pharmacyId)
        {
            var sale = await _unitOfWork.SaleRepository.GetByIdWithItemsAsync(saleId);

            if (sale == null || sale.PharmacyId != pharmacyId)
                return GeneralResult<ReadSaleDto>.NotFound("Sale not found");

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }

        public async Task<GeneralResult<IEnumerable<ReadSaleDto>>> GetAllSales(Guid pharmacyId, SaleStatus? status = null, string? search = null)
        {
            var sales = await _unitOfWork.SaleRepository.GetAllForPharmacy(pharmacyId, status, search);
            var result = sales.Select(MapSaleToDto);
            return GeneralResult<IEnumerable<ReadSaleDto>>.SuccessResult(result);
        }

        public async Task<GeneralResult<SaleStatsDto>> GetStats(Guid pharmacyId)
        {
            var stats = new SaleStatsDto
            {
                TodayTotal = await _unitOfWork.SaleRepository.GetTodayTotal(pharmacyId),
                CompletedCount = await _unitOfWork.SaleRepository.GetCompletedCount(pharmacyId),
                AverageBasket = await _unitOfWork.SaleRepository.GetAverageBasket(pharmacyId),
                CancelledCount = await _unitOfWork.SaleRepository.GetCancelledCount(pharmacyId)
            };

            return GeneralResult<SaleStatsDto>.SuccessResult(stats);
        }

        public async Task<GeneralResult<IEnumerable<SalesTrendPointDto>>> GetTrend(Guid pharmacyId, int days = 7)
        {
            if (days < 1) days = 7;
            if (days > 90) days = 90;

            var rows = await _unitOfWork.SaleRepository.GetDailyTotals(pharmacyId, days);

            var points = rows.Select(r => new SalesTrendPointDto
            {
                Date = r.Date,
                DayLabel = r.Date.ToString("ddd"),
                Total = r.Total,
                OrderCount = r.OrderCount
            });

            return GeneralResult<IEnumerable<SalesTrendPointDto>>.SuccessResult(points);
        }

        public async Task<GeneralResult<IEnumerable<CategoryMixDto>>> GetCategoryMix(Guid pharmacyId)
        {
            var rows = (await _unitOfWork.SaleRepository.GetCategoryRevenue(pharmacyId)).ToList();
            var totalRevenue = rows.Sum(r => r.Revenue);

            var result = rows
                .Select(r => new CategoryMixDto
                {
                    Category = r.Category,
                    Revenue = r.Revenue,
                    Percentage = totalRevenue > 0 ? Math.Round(r.Revenue / totalRevenue * 100, 1) : 0
                })
                .OrderByDescending(r => r.Revenue)
                .ToList();

            return GeneralResult<IEnumerable<CategoryMixDto>>.SuccessResult(result);
        }

        public async Task<GeneralResult<IEnumerable<ReadSaleDto>>> GetCustomerSales(
      Guid customerId,
      string? search = null,
      Guid? pharmacyId = null,
      SaleStatus? status = null,
      DateTime? from = null,
      DateTime? to = null,
      int page = 1,
      int pageSize = 10)
        {
            var sales = await _unitOfWork.SaleRepository.GetByCustomerIdAsync(
                customerId,
                search,
                pharmacyId,
                status,
                from,
                to,
                page,
                pageSize);

            var result = sales.Select(MapSaleToDto);

            return GeneralResult<IEnumerable<ReadSaleDto>>.SuccessResult(result);
        }

        public async Task<GeneralResult<ReadSaleDto>> GetCustomerSaleById(Guid saleId, Guid customerId)
        {
            var sale = await _unitOfWork.SaleRepository
                .GetByIdWithItemsAndCustomerIdAsync(saleId, customerId);

            if (sale == null)
                return GeneralResult<ReadSaleDto>.NotFound("Sale not found.");

            return GeneralResult<ReadSaleDto>.SuccessResult(MapSaleToDto(sale));
        }




        ////////////////////////////////
        private async Task CheckLowStockNotifications(
    IEnumerable<Guid> pharmacyMedicineIds,
    Guid pharmacyId)
        {
            var medicineIds = pharmacyMedicineIds.Distinct();

            foreach (var pharmacyMedicineId in medicineIds)
            {
                var pharmacyMedicine = await _unitOfWork
                    .PharmacyMedicineRepository
                    .GetByIdAndPharmacy(pharmacyMedicineId, pharmacyId);

                if (pharmacyMedicine == null)
                    continue;

                var availableQuantity = await _unitOfWork
                    ._batchRepository
                    .GetAvailableQuantity(pharmacyMedicineId, pharmacyId);

                if (availableQuantity < pharmacyMedicine.MinStockLevel)
                {
                    await _notificationManager.CreateLowStockNotification(
                        pharmacyId,
                        pharmacyMedicineId,
                        pharmacyMedicine.TradeNameEn,
                        pharmacyMedicine.TradeNameAr,
                        availableQuantity,
                        pharmacyMedicine.MinStockLevel);
                }
            }
        }

    }
}