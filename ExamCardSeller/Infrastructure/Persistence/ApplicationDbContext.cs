using ExamCardSeller.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExamCardSeller.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Payment entity
            modelBuilder.Entity<PurchaseRequest>(entity =>
            {
                // Specify the table name
                entity.ToTable("purchase_requests");

                // Configure indexes
                entity.HasIndex(e => e.ClientOrderReference).HasDatabaseName("IX_Client_Order_Reference").IsUnique();
                entity.HasIndex(e => e.PaymentReference).HasDatabaseName("IX_Payment_Reference");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Purchase_Request_CreatedAt");
                entity.HasIndex(e => e.PurchaserEmailId).HasDatabaseName("IX_Purchaser_Email_Id");

                entity.Property(e => e.ClientOrderReference)
                 .IsRequired()
                 .HasMaxLength(50);
                entity.Property(e => e.PurchaserEmailId)
                 .IsRequired()
                 .HasMaxLength(50);
                entity.Property(e => e.PurchaserName)
                 .IsRequired()
                 .HasMaxLength(50);
                entity.Property(e => e.RequestBody)
                 .HasColumnType("jsonb") 
                  .HasDefaultValue("{}");
                entity.Property(e => e.PurchaseContent)
                .HasColumnType("jsonb")
                 .HasDefaultValue("{}");
                entity.Property(e => e.PaymentReference)
                 .HasMaxLength(50);
                entity.Property(e => e.PaymentProcessorError)
                .HasMaxLength(500);
            });
        }

        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
    }
}
