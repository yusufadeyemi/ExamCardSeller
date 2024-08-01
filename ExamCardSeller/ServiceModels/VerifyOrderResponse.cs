using ExamCardSeller.Domain;

namespace ExamCardSeller.ServiceModels
{
    public record VerifyOrderResponse
    {
        public DateTime? PaymentDate { get; set; }

        public string? PaymentReference { get; set; }

        public required string ClientOrderRefere { get; set; }
        public PurchaseType PurchaseType { get; set; }

        public dynamic? Payload { get; set; }

        public PurchaseStatus PurchaseStatus { get; set; }
    }
}
