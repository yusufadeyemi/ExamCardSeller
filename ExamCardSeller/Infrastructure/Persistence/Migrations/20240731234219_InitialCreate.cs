using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamCardSeller.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchase_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaserEmailId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PurchaserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PurchaseType = table.Column<int>(type: "integer", nullable: false),
                    RequestBody = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    PurchaseStatus = table.Column<int>(type: "integer", nullable: false),
                    PurchaseContent = table.Column<string>(type: "jsonb", nullable: true, defaultValue: "{}"),
                    PaymentProcessorError = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaymentLink = table.Column<string>(type: "text", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientOrderReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_requests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_Order_Reference",
                table: "purchase_requests",
                column: "ClientOrderReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Reference",
                table: "purchase_requests",
                column: "PaymentReference");

            migrationBuilder.CreateIndex(
                name: "IX_Purchase_Request_CreatedAt",
                table: "purchase_requests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Purchaser_Email_Id",
                table: "purchase_requests",
                column: "PurchaserEmailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchase_requests");
        }
    }
}
