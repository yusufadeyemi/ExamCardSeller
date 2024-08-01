using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExamCardSeller.Infrastructure.Gateways
{
    public class PaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackSettings _settings;

        public PaystackService(HttpClient httpClient, PaystackSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        public async Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request)
        {
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/transaction/initialize", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseString);
                throw new PaystackException(errorResponse.Message, response.StatusCode);
            }

            return JsonSerializer.Deserialize<CreatePaymentLinkResponse>(responseString);
        }

        public async Task<VerifyPaymentResponse> VerifyPaymentAsync(string reference)
        {
            var response = await _httpClient.GetAsync($"/transaction/verify/{reference}");
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseString);
                throw new PaystackException(errorResponse.Message, response.StatusCode);
            }

            return JsonSerializer.Deserialize<VerifyPaymentResponse>(responseString);
        }
    }

    public class PaystackException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public PaystackException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class ErrorResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string[] Errors { get; set; }
    }
    public record CreatePaymentLinkRequest
    {
        [JsonPropertyName("amount")]
        public required decimal Amount { get; set; }
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "NGN";
        [JsonPropertyName("reference")]
        public required string Reference { get; set; }
        [JsonPropertyName("callback_url")]
        public required string CallbackUrl { get; set; }
    }

    public class CreatePaymentLinkResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("data")]
        public CreatePaymentData Data { get; set; }

        public class CreatePaymentData
        {
            [JsonPropertyName("authorization_url")]
            public string AuthorizationUrl { get; set; }
            [JsonPropertyName("access_code")]
            public string AccessCode { get; set; }
            [JsonPropertyName("reference")]
            public string Reference { get; set; }
        }
    }

    public class VerifyPaymentResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("data")]
        public PaystackData Data { get; set; }

        public class PaystackData
        {
            [JsonPropertyName("status")]
            public string Status { get; set; }
            [JsonPropertyName("reference")]
            public string Reference { get; set; }
            [JsonPropertyName("amount")]
            public decimal Amount { get; set; }
            [JsonPropertyName("gateway_response")]
            public string GatewayResponse { get; set; }
            [JsonPropertyName("paid_at")]
            public string PaidAt { get; set; }
            [JsonPropertyName("created_at")]
            public string CreatedAt { get; set; }
        }
    }

    public class PaystackSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; } = "https://api.paystack.co";
        public string Callback { get; set; }
    }

}

