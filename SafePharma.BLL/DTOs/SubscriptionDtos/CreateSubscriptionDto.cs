
namespace SafePharma.BLL
{
    public class CreateSubscriptionDto
    {
        public string PlanTier { get; set; }        // Starter | Professional | Enterprise
        public string BillingCycle { get; set; }     // monthly | yearly

        public PharmacyInfoDto Pharmacy { get; set; }
        public PrimaryContactDto PrimaryContact { get; set; }
    }
}
