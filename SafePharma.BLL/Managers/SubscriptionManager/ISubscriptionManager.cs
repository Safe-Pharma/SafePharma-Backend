using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface ISubscriptionManager
    {
        Task<GeneralResult<SubscriptionReadDto>> CreateSubscription(CreateSubscriptionDto dto);
        Task<IEnumerable<SubscriptionReadDto>> GetAllSubscriptions();
        Task<GeneralResult<SubscriptionReadDto>> GetSubscriptionById(Guid id);
        Task<GeneralResult<SubscriptionReadDto>> UpdateSubscription(Guid id, UpdateSubscriptionDto dto);
        Task<GeneralResult> CancelSubscription(Guid id, Guid adminId);
    }
}