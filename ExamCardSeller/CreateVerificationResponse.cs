namespace ExamCardSeller.ServiceModels
{
    public class CreateVerificationResponse
    {
        public required string PaymentLink { get; set; }
        public required string ClientOrderReference { get; set; }
    }

    public class ConfirmOrderResponse
    {
        public required string? Token { get; set; }
        public required bool Success { get; set; }
    }
}
