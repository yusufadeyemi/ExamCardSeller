using ExamCardSeller.ServiceModels;

namespace ExamCardSeller.Services
{
    public interface IPurchaseService
    {
        Task<CreateVerificationResponse> MakeOrder(CreateVerificationRequest request);

        Task<ConfirmOrderResponse> VerifyOrder(string clientOrderReference);
    }

}
