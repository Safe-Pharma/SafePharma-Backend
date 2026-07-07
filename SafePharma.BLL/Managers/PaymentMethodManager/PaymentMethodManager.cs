using System.Text.Json;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class PaymentMethodManager : IPaymentMethodManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentMethodManager(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<PaymentMethodReadDto>> GetAllMethods()
        {
            var methods = await _unitOfWork.PaymentMethodRepository.GetAll();
            return methods.OrderBy(m => m.SortOrder).Select(MapToReadDto);
        }

        public async Task<GeneralResult<PaymentMethodReadDto>> CreateMethod(PaymentMethodUpsertDto dto)
        {
            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                MethodName = dto.MethodName,
                FieldsJson = JsonSerializer.Serialize(dto.Fields),
                IsActive = dto.IsActive,
                SortOrder = dto.SortOrder
            };

            _unitOfWork.PaymentMethodRepository.Add(method);
            await _unitOfWork.SaveAsync();

            return GeneralResult<PaymentMethodReadDto>.SuccessResult(MapToReadDto(method), "Payment method created.");
        }

        public async Task<GeneralResult<PaymentMethodReadDto>> UpdateMethod(Guid id, PaymentMethodUpsertDto dto)
        {
            var method = await _unitOfWork.PaymentMethodRepository.GetById(id);
            if (method == null)
                return GeneralResult<PaymentMethodReadDto>.NotFound("Payment method not found.");

            method.MethodName = dto.MethodName;
            method.FieldsJson = JsonSerializer.Serialize(dto.Fields);
            method.IsActive = dto.IsActive;
            method.SortOrder = dto.SortOrder;

            await _unitOfWork.SaveAsync();
            return GeneralResult<PaymentMethodReadDto>.SuccessResult(MapToReadDto(method), "Payment method updated.");
        }

        public async Task<GeneralResult> DeleteMethod(Guid id)
        {
            var method = await _unitOfWork.PaymentMethodRepository.GetById(id);
            if (method == null)
                return GeneralResult.NotFound("Payment method not found.");

            method.IsActive = false; // soft-disable — receipts already submitted may still reference this method by name
            await _unitOfWork.SaveAsync();
            return GeneralResult.SuccessResult("Payment method disabled.");
        }

        private static PaymentMethodReadDto MapToReadDto(PaymentMethod method) => new()
        {
            Id = method.Id,
            MethodName = method.MethodName,
            IsActive = method.IsActive,
            SortOrder = method.SortOrder,
            Fields = JsonSerializer.Deserialize<List<PaymentMethodFieldDto>>(method.FieldsJson) ?? new()
        };
    }
}