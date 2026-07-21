using SafePharma.Common;
using System.Net.Http.Json;

namespace SafePharma.BLL
{
    public class WhatsAppBaileysChannel : IOtpDeliveryChannel
    {
        private readonly HttpClient _http;
        public string ChannelName => "WhatsApp";

        public WhatsAppBaileysChannel(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResult> SendAsync(string phoneNumber, string otpCode, CancellationToken ct = default)
        {
            Console.WriteLine($"[WhatsAppBaileysChannel] Calling {_http.BaseAddress}send-otp for phone {phoneNumber}");

            try
            {
                var payload = new { phone = phoneNumber, otp = otpCode };
                var response = await _http.PostAsJsonAsync("/send-otp", payload, ct);
                    
                return response.IsSuccessStatusCode
                    ? GeneralResult.SuccessResult("Sent via WhatsApp")
                    : GeneralResult.FailResult("WhatsApp send failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhatsAppBaileysChannel ERROR]: {ex.GetType().Name} - {ex.Message}");

                return GeneralResult.FailResult($"WhatsApp error: {ex.Message}");
            }
        }
    }
}