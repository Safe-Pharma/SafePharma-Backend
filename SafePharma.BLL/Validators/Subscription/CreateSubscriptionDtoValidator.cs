using FluentValidation;

namespace SafePharma.BLL
{
    public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
    {
        private static readonly string[] ValidPlanTiers = { "Starter", "Professional", "Enterprise" };
        private static readonly string[] ValidBillingCycles = { "monthly", "yearly" };

        public CreateSubscriptionDtoValidator()
        {
            RuleFor(x => x.PlanTier)
                .NotEmpty().WithMessage("Plan tier is required.")
                .Must(p => ValidPlanTiers.Contains(p))
                .WithMessage($"Plan tier must be one of: {string.Join(", ", ValidPlanTiers)}.");

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
    }
}