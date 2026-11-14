using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteCategoryPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Categories_CategoryId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringTransactions_Categories_CategoryId",
                table: "RecurringTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_RecurringTransactions_CategoryId",
                table: "RecurringTransactions");

            migrationBuilder.DropIndex(
                name: "IX_RecurringTransactions_UserId",
                table: "RecurringTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_CategoryId",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CategoryType",
                table: "Categories",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_UserId_Id",
                table: "Categories",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_CategoryId",
                table: "Transactions",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transactions_CategoryCombination",
                table: "Transactions",
                sql: "(\"TransactionType\" IN (1, 2) AND \"CategoryId\" IS NOT NULL)\r\nOR\r\n(\"TransactionType\" = 3 AND \"CategoryId\" IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId_CategoryId",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_CategoryCombination",
                table: "RecurringTransactions",
                sql: "(\"TransactionType\" IN (1, 2) AND \"CategoryId\" IS NOT NULL)\r\nOR\r\n(\"TransactionType\" = 3 AND \"CategoryId\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_CategoryType_Valid",
                table: "Categories",
                sql: "\"CategoryType\" IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId_CategoryId",
                table: "Budgets",
                columns: new[] { "UserId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Categories_UserId_CategoryId",
                table: "Budgets",
                columns: new[] { "UserId", "CategoryId" },
                principalTable: "Categories",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringTransactions_Categories_UserId_CategoryId",
                table: "RecurringTransactions",
                columns: new[] { "UserId", "CategoryId" },
                principalTable: "Categories",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_UserId_CategoryId",
                table: "Transactions",
                columns: new[] { "UserId", "CategoryId" },
                principalTable: "Categories",
                principalColumns: new[] { "UserId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Categories_UserId_CategoryId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringTransactions_Categories_UserId_CategoryId",
                table: "RecurringTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_UserId_CategoryId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_CategoryId",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Transactions_CategoryCombination",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_RecurringTransactions_UserId_CategoryId",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_CategoryCombination",
                table: "RecurringTransactions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_UserId_Id",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_CategoryType_Valid",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_UserId_CategoryId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_CategoryId",
                table: "RecurringTransactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTransactions_UserId",
                table: "RecurringTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CategoryId",
                table: "Budgets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Categories_CategoryId",
                table: "Budgets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringTransactions_Categories_CategoryId",
                table: "RecurringTransactions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
