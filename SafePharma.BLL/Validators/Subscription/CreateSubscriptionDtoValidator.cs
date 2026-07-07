using FluentValidation;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
    {
        private static readonly string[] ValidBillingCycles = { "monthly", "yearly" };
        private readonly ISubscriptionPlanRepository _planRepository;

        public CreateSubscriptionDtoValidator(ISubscriptionPlanRepository planRepository)
        {
            _planRepository = planRepository;

            RuleFor(x => x.PlanTier)
                .NotEmpty().WithMessage("Plan tier is required.")
                .MustAsync(BeAnActivePlan)
                .WithMessage("Selected plan is not available.");

            RuleFor(x => x.BillingCycle)
                .NotEmpty().WithMessage("Billing cycle is required.")
                .Must(b => ValidBillingCycles.Contains(b))
                .WithMessage($"Billing cycle must be one of: {string.Join(", ", ValidBillingCycles)}.");

            RuleFor(x => x.Pharmacy)
                .NotNull().WithMessage("Pharmacy information is required.")
                .SetValidator(new PharmacyInfoDtoValidator());

            RuleFor(x => x.PrimaryContact)
                .NotNull().WithMessage("Primary contact information is required.")
                .SetValidator(new PrimaryContactDtoValidator());
        }

        private async Task<bool> BeAnActivePlan(string tier, CancellationToken ct)
        {
            var plan = await _planRepository.GetByTier(tier);
            return plan is { IsActive: true };
        }
    }
}