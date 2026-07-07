namespace SafePharma.Common
{
    public class PaymentSettings
    {
        public string Currency { get; set; } = "EGP";
        public List<PaymentMethodSetting> Methods { get; set; } = new();
        public Dictionary<string, decimal> MonthlyPrices { get; set; } = new();
        public Dictionary<string, decimal> YearlyPrices { get; set; } = new();
    }

    public class PaymentMethodSetting
    {
        public string MethodName { get; set; }
        public string AccountName { get; set; }
        public string AccountDetails { get; set; }
    }
}