using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface IPaymentMethodManager
    {
        Task<IEnumerable<PaymentMethodReadDto>> GetAllMethods();
        Task<GeneralResult<PaymentMethodReadDto>> CreateMethod(PaymentMethodUpsertDto dto);
        Task<GeneralResult<PaymentMethodReadDto>> UpdateMethod(Guid id, PaymentMethodUpsertDto dto);
        Task<GeneralResult> DeleteMethod(Guid id);
    }
}