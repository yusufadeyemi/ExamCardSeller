namespace ExamCardSeller.Domain
{
    public class PurchaseRequest : BaseEntity
    {
        public string PurchaserEmailId { get; set; }
        public string PurchaserName { get; set; }
        public PurchaseType PurchaseType { get; set; }
        public decimal Amount { get; set; }

        public string RequestBody { get; set; }

        public PurchaseStatus PurchaseStatus { get; set; } = PurchaseStatus.Pending;

        public string? PurchaseContent { get; set; }

        public string? PaymentProcessorError { get; set; }
        public string? PaymentLink { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? PaymentReference { get; set; }

        public required string ClientOrderReference { get; set; }

    }
    public enum PurchaseType
    {
        VerificationToken =1,
        WaecPin =2,
        NecoPin = 3,
        NabtebPin = 4,
    }

    public enum PurchaseStatus
    {
        Pending = 0,
        PaymentFailed = 1,
        PaymentLinkCreated = 2,
        PaymentSuccess = 3,
        PaymentProcessorError = 4,
        OrderDelivered = 5,
    }
}
