namespace SafePharma.BLL
{
    public class PaymentMethodFieldDto
    {
        public string Label { get; set; }   // e.g. "Handle", "IBAN", "Wallet"
        public string Value { get; set; }   // e.g. "@safepharma", "AE07 0331 ..."
    }
}