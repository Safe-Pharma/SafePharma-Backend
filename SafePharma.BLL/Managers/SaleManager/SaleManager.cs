using SafePharma.Common;
using SafePharma.DAL;
using System.Threading.Tasks;

namespace SafePharma.BLL
{
    public class SaleManager : ISaleManager
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly ICutomerRepository _customerRepository; =========----To Do----=========

        public SaleManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<ReadSaleDto>> CreateSale(CreateSaleDto createSaleDto, Guid pharmacyId, Guid userId)
        {
            if (createSaleDto == null)
            {
                return null;
            }

            var sale = new Sale
            {
                PharmacyId = pharmacyId,
                ApplicationUserId = userId,
                //CustomerId = CustomerId,  
                PaymentMethod = createSaleDto.PaymentMethod,
                Tax = createSaleDto.Tax,
                Discount = createSaleDto.Discount,
                Total = createSaleDto.Total,
                AmountPaid = createSaleDto.AmountPaid,
                Status = createSaleDto.Status,
                CreatedAt = DateTime.UtcNow,
                SaleItems = createSaleDto.Items
                    .Select(item => new SaleItem
                    {
                        PharmacyMedicineId = item.PharmacyMedicineId,
                        CustomerId = item.CustomerId,
                        BatchId = item.BatchId,
                        Quantity = item.Quantity,
                        LineTotal = item.LineTotal,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        TaxAmount = item.TaxAmount

                    }).ToList()
            };

            _unitOfWork.SaleRepository.Add(sale);
            await _unitOfWork.SaveAsync();

            //var savedSale = await _unitOfWork.

            var result = new ReadSaleDto
            {
                Id = sale.Id,
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
                    MedicineName = item.PharmacyMedicine.Medicine.TradeNameEn,

                    CustomerId = item.CustomerId ?? Guid.Empty,
                    CustomerName = "", // Navigation Customer

                    BatchId = item.BatchId,
                    BatchNumber = item.Batch.BatchNumber,

                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TaxAmount = item.TaxAmount,
                    LineTotal = item.LineTotal

                }).ToList()
            };

            return GeneralResult<ReadSaleDto>.SuccessResult(result);
        }
    }
}
