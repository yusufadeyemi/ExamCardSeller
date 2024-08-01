using ExamCardSeller.Infrastructure.Persistence.Repositories;
using ExamCardSeller.ServiceModels;
using FluentValidation;

namespace ExamCardSeller.Validators
{

public class CreatePurchaseRequestValidator: AbstractValidator<CreateVerificationRequest>
    {
        public CreatePurchaseRequestValidator(IPurchaseRequestRepository repository)
        {
            RuleFor(order => order.ClientOrderReference)
                .NotEmpty().WithMessage("client order reference is required.")
                .MaximumLength(50).WithMessage("client order reference must not exceed 50 characters.")
                .MustAsync(async (clientOrderReference, cancellation) => !await repository.AnyAsync(clientOrderReference))
            .WithMessage("client order reference already exists.");

        }
    }

}
