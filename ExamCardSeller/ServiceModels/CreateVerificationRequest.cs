using ExamCardSeller.Domain;

namespace ExamCardSeller.ServiceModels
{
    public record CreateVerificationRequest
    {
        public required string PurchaserEmailId { get; set; }
        public required string PurchaserName { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public required string ClientOrderReference { get; set; }

        public required string CallbackUrl { get; set; }
    }
}
