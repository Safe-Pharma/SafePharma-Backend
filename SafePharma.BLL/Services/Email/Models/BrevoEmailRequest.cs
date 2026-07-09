namespace SafePharma.BLL
{
    public class BrevoEmailRequest
    {
        public BrevoSender Sender { get; set; } = default!;
        public List<BrevoRecipient> To { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
    }
}
