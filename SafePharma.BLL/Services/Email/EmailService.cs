using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SafePharma.Common;



namespace SafePharma.BLL
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly EmailSettings _settings;

        public EmailService(
            HttpClient httpClient,
            IOptions<EmailSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string htmlContent)
        {
            var request = new BrevoEmailRequest
            {
                Sender = new BrevoSender
                {
                    Name = _settings.SenderName,
                    Email = _settings.SenderEmail
                },
                To =
                [
                    new BrevoRecipient
                {
                    Email = to
                }
                ],
                Subject = subject,
                HtmlContent = htmlContent
            };

            using var message = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.brevo.com/v3/smtp/email");

            message.Headers.Add("api-key", _settings.ApiKey);

            message.Content = JsonContent.Create(
                request,
                options: new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            var response = await _httpClient.SendAsync(message);

            response.EnsureSuccessStatusCode();
        }
    }
}
