using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class MedicineSearchService : IMedicineSearchService
    {
        private readonly IPharmacyMedicineRepository _pharmacyMedicineRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MedicineSearchService(
            IPharmacyMedicineRepository pharmacyMedicineRepository,
            IUnitOfWork unitOfWork)
        {
            _pharmacyMedicineRepository = pharmacyMedicineRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResult<PagedResult<MedicineSearchResultDto>>> SearchAsync(
            Guid pharmacyId, MedicineSearchRequestDto requestDto)
        {
            var (items, totalCount) = await _pharmacyMedicineRepository.SearchAsync(
                pharmacyId,
                requestDto.Query,
                requestDto.PageNumber,
                requestDto.PageSize);

            var itemsList = items.ToList();

            var metadata = BuildMetadata(
                requestDto.PageNumber,
                requestDto.PageSize,
                totalCount);

            if (!itemsList.Any())
            {
                return GeneralResult<PagedResult<MedicineSearchResultDto>>.SuccessResult(
                    new PagedResult<MedicineSearchResultDto>
                    {
                        Items = new List<MedicineSearchResultDto>(),
                        Metadata = metadata
                    });
            }

            // FIX: Batch.MedicineId is actually a FK to PharmacyMedicine.Id
            // (see the [ForeignKey("Medicine")] navigation typed as PharmacyMedicine in Batch.cs),
            // so we must pass PharmacyMedicine.Id here, not the catalog Medicine.Id.
            var pharmacyMedicineIds = itemsList
                .Select(pm => pm.Id)
                .ToList();

            var stockAggregates = await _unitOfWork
                ._batchRepository
                .GetStockAggregates(pharmacyMedicineIds);

            var stockMap = stockAggregates
                .ToDictionary(
                    s => s.PharmacyMedicineId,
                    s => s.AvailableQuantity
                );

            var resultDtos = itemsList
                .Select(pm => new MedicineSearchResultDto
                {
                    PharmacyMedicineId = pm.Id,

                    TradeNameAr = pm.TradeNameAr,

                    TradeNameEn = pm.TradeNameEn,

                    ScientificName = pm.ScientificName,

                    Barcode = pm.PharmacyBarcodes?
                        .FirstOrDefault(b => b.IsPrimary)?
                        .Barcode,

                    SellingPrice = pm.SellingPrice,

                    // FIX: look up by pm.Id (PharmacyMedicine.Id), matching the key
                    // produced by GetStockAggregates / stockMap above.
                    StockQuantity = stockMap.TryGetValue(pm.Id, out var qty)
                        ? qty
                        : 0

                })
                .ToList();

            var pagedResult = new PagedResult<MedicineSearchResultDto>
            {
                Items = resultDtos,
                Metadata = metadata
            };

            return GeneralResult<PagedResult<MedicineSearchResultDto>>
                .SuccessResult(pagedResult);
        }

        private static PaginationMetaData BuildMetadata(
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            return new PaginationMetaData
            {
                CurrentPage = pageNumber,

                PageSize = pageSize,

                TotalCount = totalCount,

                TotalPages = totalPages,

                HasNext = pageNumber < totalPages,

                HasPrev = pageNumber > 1
            };
        }
    }
}