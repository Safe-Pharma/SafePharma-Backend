using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface ISubscriptionManager
    {
        Task<GeneralResult<SubscriptionReadDto>> CreateSubscription(CreateSubscriptionDto dto);
    }
}