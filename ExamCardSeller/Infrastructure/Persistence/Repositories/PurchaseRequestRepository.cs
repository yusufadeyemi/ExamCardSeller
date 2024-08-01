using ExamCardSeller.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExamCardSeller.Infrastructure.Persistence.Repositories
{
    public class PurchaseRequestRepository(ApplicationDbContext context) : IPurchaseRequestRepository
    {
        public async Task AddAsync(PurchaseRequest purchaseRequest)
        {
            context.PurchaseRequests.Add(purchaseRequest);
            await context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(string clientOrderReference)
        {
            return await context.PurchaseRequests.AnyAsync(p => p.ClientOrderReference == clientOrderReference);
        }

        public async Task<PurchaseRequest?> FindAsync(Guid purchaseId)
        {
            return await context.PurchaseRequests.FindAsync(purchaseId);
        }

        public async Task<PurchaseRequest?> FindByClientReferenceAsync(string clientOrderReference)
        {
            return await context.PurchaseRequests.SingleOrDefaultAsync(p => p.ClientOrderReference == clientOrderReference);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
