using ExamCardSeller.Domain;

namespace ExamCardSeller.Infrastructure.Persistence.Repositories
{
    public interface IPurchaseRequestRepository
    {
        Task<PurchaseRequest?> FindAsync(Guid purchaseId);
        Task<PurchaseRequest?> FindByClientReferenceAsync(string clientOrderReference);
        Task AddAsync(PurchaseRequest purchaseRequest);
        Task<bool> AnyAsync(string clientOrderReference);
        Task SaveChangesAsync();
    }
}
