using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PurchaseOrderManager : IPurchaseOrderManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<PurchaseOrderReadDto>> CreateAsync(PurchaseOrderCreateDto createDto, Guid pharmacyId)
        {
            var pharmacySuppliers = await _unitOfWork.SupplierRepository.GetAllForPharmacy(pharmacyId);
            var supplier = pharmacySuppliers.FirstOrDefault(s => s.Id == createDto.SupplierId);

            if (supplier is null)
                return GeneralResult<PurchaseOrderReadDto>.NotFound("Supplier not found.");

            var po = new PurchaseOrder
            {
                PharmacyId = pharmacyId,
                OrderDate = createDto.OrderDate,
                ExpectedDate = createDto.ExpectedDate,
                OrderNumber = $"PO-{DateTime.UtcNow:yyMMddHHmmss}",
                SupplierId = createDto.SupplierId,
                Status = "Open",
                TotalAmount = createDto.Items.Sum(i => i.QuantityOrdered * i.UnitPrice),
                Items = createDto.Items.Select(i => new PurchaseOrderItem
                {
                    PharmacyMedicineId = i.PharmacyMedicineId,
                    QuantityOrdered = i.QuantityOrdered,
                    UnitPrice = i.UnitPrice,
                    SellingPrice = i.SellingPrice
                }).ToList()
            };

            _unitOfWork.PurchaseOrderRepository.Add(po);
            await _unitOfWork.SaveAsync();

            var savedOrder = await _unitOfWork.PurchaseOrderRepository.GetByIdWithDetailsAsync(po.Id);

            return GeneralResult<PurchaseOrderReadDto>.SuccessResult(new PurchaseOrderReadDto
            {
                OrderNumber = savedOrder.OrderNumber,
                OrderDate = savedOrder.OrderDate,
                ExpectedDate = savedOrder.ExpectedDate,
                Status = savedOrder.Status,
                TotalAmount = savedOrder.TotalAmount,
                Lines = savedOrder.Items.Count,
                SupplierName = supplier.Name,
                Items = savedOrder.Items.Select(i => new PurchaseOrderItemReadDto
                 {
                    MedicineName = i.PharmacyMedicine.Medicine.TradeNameEn,
                    QuantityOrdered = i.QuantityOrdered,
                    UnitPrice = i.UnitPrice,
                    SellingPrice = i.SellingPrice
                 }).ToList()
            });
        }
        public async Task<GeneralResult<IEnumerable<PurchaseOrderReadDto>>> GetAllAsync(Guid pharmacyId)
        {
            var orders = await _unitOfWork.PurchaseOrderRepository.GetAllAsync(pharmacyId);

            var dtos = orders.Select(po => new PurchaseOrderReadDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                OrderDate = po.OrderDate,
                ExpectedDate = po.ExpectedDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                Lines = po.Items.Count,
                SupplierName = po.Supplier?.Name ?? "",
                Items = po.Items.Select(i => new PurchaseOrderItemReadDto
                {
                    Id = i.Id,
                    MedicineName = i.PharmacyMedicine.Medicine.TradeNameEn,
                    QuantityOrdered = i.QuantityOrdered,
                    UnitPrice = i.UnitPrice,
                    SellingPrice = i.SellingPrice
                }).ToList()

            });

            return GeneralResult<IEnumerable<PurchaseOrderReadDto>>.SuccessResult(dtos);
        }
    }
}
