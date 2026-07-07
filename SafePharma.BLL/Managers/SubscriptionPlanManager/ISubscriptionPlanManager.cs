using SafePharma.Common;

namespace SafePharma.BLL
{
    public interface ISubscriptionPlanManager
    {
        Task<IEnumerable<SubscriptionPlanReadDto>> GetActivePlans();
        Task<IEnumerable<SubscriptionPlanReadDto>> GetAllPlans();
        Task<GeneralResult<SubscriptionPlanReadDto>> CreatePlan(SubscriptionPlanUpsertDto dto);
        Task<GeneralResult<SubscriptionPlanReadDto>> UpdatePlan(Guid id, SubscriptionPlanUpsertDto dto);
        Task<GeneralResult> DeletePlan(Guid id);
    }
}