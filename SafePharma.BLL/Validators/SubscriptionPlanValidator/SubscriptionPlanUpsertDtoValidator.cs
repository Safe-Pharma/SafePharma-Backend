using FluentValidation;

namespace SafePharma.BLL
{
    public class SubscriptionPlanUpsertDtoValidator : AbstractValidator<SubscriptionPlanUpsertDto>
    {
        public SubscriptionPlanUpsertDtoValidator()
        {
            RuleFor(x => x.Tier).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.MonthlyPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.YearlyPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Currency).NotEmpty();
        }
    }

}
