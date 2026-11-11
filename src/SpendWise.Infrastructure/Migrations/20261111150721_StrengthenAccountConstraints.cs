using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StrengthenAccountConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_AccountType_Valid",
                table: "Accounts",
                sql: "\"AccountType\" IN (1, 2, 3, 4, 5, 6, 7, 8)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_CreditCardBalance_WithinLimit",
                table: "Accounts",
                sql: "\"AccountType\" <> 4\r\nOR\r\n(\r\n    \"CreditLimit_Amount\" IS NOT NULL\r\n    AND \"OpeningBalance_Amount\" <= \"CreditLimit_Amount\"\r\n    AND \"Balance_Amount\" <= \"CreditLimit_Amount\"\r\n)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_CreditLimit_AccountType",
                table: "Accounts",
                sql: "(\r\n    \"AccountType\" = 4\r\n    AND \"CreditLimit_Amount\" IS NOT NULL\r\n    AND \"CreditLimit_Currency\" = 'USD'\r\n)\r\nOR\r\n(\r\n    \"AccountType\" <> 4\r\n    AND \"CreditLimit_Amount\" IS NULL\r\n    AND \"CreditLimit_Currency\" IS NULL\r\n)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Name_NotBlank",
                table: "Accounts",
                sql: "length(btrim(\"AccountName\")) > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_AccountType_Valid",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_CreditCardBalance_WithinLimit",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_CreditLimit_AccountType",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_Name_NotBlank",
                table: "Accounts");
        }
    }
}
