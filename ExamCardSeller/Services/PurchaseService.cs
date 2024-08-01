using ExamCardSeller.Domain;
using ExamCardSeller.Infrastructure.Gateways;
using ExamCardSeller.Infrastructure.Persistence.Repositories;
using ExamCardSeller.ServiceModels;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

namespace ExamCardSeller.Services
{
    public class PurchaseService(IPurchaseRequestRepository purchaseRequestRepository, PaystackService paystack, IValidator<CreateVerificationRequest> validator ) : IPurchaseService
    {
        private async Task<ValidationResult> ValidateOrder(CreateVerificationRequest request)
        {
            return await validator.ValidateAsync(request);
        }
        public async Task<CreateVerificationResponse> MakeOrder(CreateVerificationRequest request)
        {
            // Validation on request

            // Not emptpy or null
            // Not exist before
            var validationResult = await ValidateOrder(request);

            if (!validationResult.IsValid)
            {
                // Handle validation failures
                //foreach (var error in validationResult.Errors)
                //{
                //    Console.WriteLine($"Property {error.PropertyName} failed validation. Error: {error.ErrorMessage}");
                //}

                throw new ValidationException(validationResult.Errors);
            }
            // Paystack Create Link
            // amount in kobo
            var createLinkRequest = new CreatePaymentLinkRequest
            {
                Amount = 30000,
                Email = request.PurchaserEmailId,
                Reference = request.ClientOrderReference,
                CallbackUrl = request.CallbackUrl
            };
            var createLinkResponse = await paystack.CreatePaymentLinkAsync(createLinkRequest);
            if (!createLinkResponse.Status)
                throw new BadHttpRequestException($"Unable to create payment link - {createLinkResponse.Message}");
            // Save Order
            var purchaseRequest = new PurchaseRequest
            {
                PurchaserEmailId = request.PurchaserEmailId,
                ClientOrderReference = request.ClientOrderReference,
                Amount = createLinkRequest.Amount,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.PurchaserEmailId,
                PurchaserName = request.PurchaserName,
                PurchaseType = PurchaseType.VerificationToken,
                RequestBody = JsonSerializer.Serialize(new { }),
                PaymentLink = createLinkResponse.Data.AuthorizationUrl
            };
            await purchaseRequestRepository.AddAsync(purchaseRequest);
            return new CreateVerificationResponse
            {
                ClientOrderReference = purchaseRequest.ClientOrderReference,
                PaymentLink = createLinkResponse.Data.AuthorizationUrl,
            };
        }

        public async Task<ConfirmOrderResponse> VerifyOrder(string clientOrderReference)
        {
            // check db
            var order = await purchaseRequestRepository.FindByClientReferenceAsync(clientOrderReference);
            if (order == null)
                throw new BadHttpRequestException($"unknown reference {clientOrderReference}");

            if(order.PurchaseStatus is PurchaseStatus.PaymentSuccess or PurchaseStatus.OrderDelivered)
                return new ConfirmOrderResponse { Success = true, Token = clientOrderReference };
            // check paystack
            var paystackRes =  await paystack.VerifyPaymentAsync(clientOrderReference);
            if(paystackRes?.Data?.Status == "success")
            {
                order.PurchaseStatus = PurchaseStatus.PaymentSuccess;
                order.PaymentDate = DateTime.Now;
                order.PurchaseContent = JsonSerializer.Serialize(new { Token = clientOrderReference });
                await purchaseRequestRepository.SaveChangesAsync();
                return new ConfirmOrderResponse { Success = true, Token = clientOrderReference };
            }
            return new ConfirmOrderResponse { Success = false, Token = null };


        }
    }
}
