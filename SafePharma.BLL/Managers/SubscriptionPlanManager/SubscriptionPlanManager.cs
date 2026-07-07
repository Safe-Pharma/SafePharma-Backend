using System.Text.Json;
using SafePharma.Common;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class SubscriptionPlanManager : ISubscriptionPlanManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionPlanManager(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<SubscriptionPlanReadDto>> GetActivePlans()
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetActiveOrdered();
            return plans.Select(MapToReadDto);
        }

        public async Task<IEnumerable<SubscriptionPlanReadDto>> GetAllPlans()
        {
            var plans = await _unitOfWork.SubscriptionPlanRepository.GetAll();
            return plans.OrderBy(p => p.SortOrder).Select(MapToReadDto);
        }

        public async Task<GeneralResult<SubscriptionPlanReadDto>> CreatePlan(SubscriptionPlanUpsertDto dto)
        {
            if (await _unitOfWork.SubscriptionPlanRepository.GetByTier(dto.Tier) != null)
                return GeneralResult<SubscriptionPlanReadDto>.FailResult($"A plan for tier '{dto.Tier}' already exists.");

            var plan = new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Tier = dto.Tier,
                DisplayName = dto.DisplayName,
                MonthlyPrice = dto.MonthlyPrice,
                YearlyPrice = dto.YearlyPrice,
                Currency = dto.Currency,
                FeaturesJson = JsonSerializer.Serialize(dto.Features),
                IsActive = dto.IsActive,
                SortOrder = dto.SortOrder
            };

            _unitOfWork.SubscriptionPlanRepository.Add(plan);
            await _unitOfWork.SaveAsync();

            return GeneralResult<SubscriptionPlanReadDto>.SuccessResult(MapToReadDto(plan), "Plan created.");
        }

        public async Task<GeneralResult<SubscriptionPlanReadDto>> UpdatePlan(Guid id, SubscriptionPlanUpsertDto dto)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetById(id);
            if (plan == null)
                return GeneralResult<SubscriptionPlanReadDto>.NotFound("Plan not found.");

            plan.DisplayName = dto.DisplayName;
            plan.MonthlyPrice = dto.MonthlyPrice;
            plan.YearlyPrice = dto.YearlyPrice;
            plan.Currency = dto.Currency;
            plan.FeaturesJson = JsonSerializer.Serialize(dto.Features);
            plan.IsActive = dto.IsActive;
            plan.SortOrder = dto.SortOrder;
            // Tier is intentionally not editable — it's the key that links to existing subscriptions.

            await _unitOfWork.SaveAsync();
            return GeneralResult<SubscriptionPlanReadDto>.SuccessResult(MapToReadDto(plan), "Plan updated.");
        }

        public async Task<GeneralResult> DeletePlan(Guid id)
        {
            var plan = await _unitOfWork.SubscriptionPlanRepository.GetById(id);
            if (plan == null)
                return GeneralResult.NotFound("Plan not found.");

            // Soft-disable instead of hard delete — existing subscriptions still reference this Tier by name.
            plan.IsActive = false;
            await _unitOfWork.SaveAsync();
            return GeneralResult.SuccessResult("Plan disabled.");
        }

        private static SubscriptionPlanReadDto MapToReadDto(SubscriptionPlan plan) => new()
        {
            Id = plan.Id,
            Tier = plan.Tier,
            DisplayName = plan.DisplayName,
            MonthlyPrice = plan.MonthlyPrice,
            YearlyPrice = plan.YearlyPrice,
            Currency = plan.Currency,
            Features = JsonSerializer.Deserialize<List<string>>(plan.FeaturesJson) ?? new(),
            IsActive = plan.IsActive,
            SortOrder = plan.SortOrder
        };
    }
}