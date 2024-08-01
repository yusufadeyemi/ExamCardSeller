﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamCardSeller.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ColumnTypesAndAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "purchase_requests",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "purchase_requests");
        }
    }
}
