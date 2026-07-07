using FluentValidation;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class UpdateSubscriptionDtoValidator : AbstractValidator<UpdateSubscriptionDto>
    {
        private static readonly string[] ValidBillingCycles = { "monthly", "yearly" };
        private readonly ISubscriptionPlanRepository _planRepository;

        public UpdateSubscriptionDtoValidator(ISubscriptionPlanRepository planRepository)
        {
            _planRepository = planRepository;

            RuleFor(x => x.PlanTier)
                .NotEmpty().WithMessage("Plan tier is required.")
                .MustAsync(async (tier, ct) => await _planRepository.GetByTier(tier) is { IsActive: true })
                .WithMessage("Selected plan is not available.");

            RuleFor(x => x.BillingCycle)
                .NotEmpty().WithMessage("Billing cycle is required.")
                .Must(b => ValidBillingCycles.Contains(b))
                .WithMessage($"Billing cycle must be one of: {string.Join(", ", ValidBillingCycles)}.");
        }
    }
}