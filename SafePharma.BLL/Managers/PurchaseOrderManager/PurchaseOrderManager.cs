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
                    MedicineId = i.MedicineId,
                    QuantityOrdered = i.QuantityOrdered,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _unitOfWork.PurchaseOrderRepository.Add(po);
            await _unitOfWork.SaveAsync();

            return GeneralResult<PurchaseOrderReadDto>.SuccessResult(new PurchaseOrderReadDto
            {
                OrderNumber = po.OrderNumber,
                OrderDate = po.OrderDate,
                ExpectedDate = po.ExpectedDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                Lines = po.Items.Count,
                SupplierName = supplier.Name
            });
        }
        public async Task<GeneralResult<IEnumerable<PurchaseOrderReadDto>>> GetAllAsync(Guid pharmacyId)
        {
            var orders = await _unitOfWork.PurchaseOrderRepository.GetAllWithSupplierAsync(pharmacyId);

            var dtos = orders.Select(po => new PurchaseOrderReadDto
            {
                OrderNumber = po.OrderNumber,
                OrderDate = po.OrderDate,
                ExpectedDate = po.ExpectedDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                Lines = po.Items.Count,
                SupplierName = po.Supplier?.Name ?? ""
            });

            return GeneralResult<IEnumerable<PurchaseOrderReadDto>>.SuccessResult(dtos);
        }
    }
}
