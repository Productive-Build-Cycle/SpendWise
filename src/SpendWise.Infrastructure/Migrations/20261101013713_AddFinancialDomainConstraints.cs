using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialDomainConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "TransactionType",
                table: "Transactions",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "TransactionType",
                table: "RecurringTransactions",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "Frequency",
                table: "RecurringTransactions",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReleaseDate",
                table: "GoalContributionReleases",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "Accounts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transactions_AccountCombination",
                table: "Transactions",
                sql: "([TransactionType] = 1 AND [SourceAccountId] IS NULL AND [DestinationAccountId] IS NOT NULL) OR ([TransactionType] = 2 AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NULL) OR ([TransactionType] = 3 AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transactions_Amount_Positive",
                table: "Transactions",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transactions_DifferentAccounts",
                table: "Transactions",
                sql: "[SourceAccountId] IS NULL OR [DestinationAccountId] IS NULL OR [SourceAccountId] <> [DestinationAccountId]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_AccountCombination",
                table: "RecurringTransactions",
                sql: "([TransactionType] = 1 AND [SourceAccountId] IS NULL AND [DestinationAccountId] IS NOT NULL) OR ([TransactionType] = 2 AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NULL) OR ([TransactionType] = 3 AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Amount_Positive",
                table: "RecurringTransactions",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_DifferentAccounts",
                table: "RecurringTransactions",
                sql: "[SourceAccountId] IS NULL OR [DestinationAccountId] IS NULL OR [SourceAccountId] <> [DestinationAccountId]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_ValidDates",
                table: "RecurringTransactions",
                sql: "[StartDate] <= [NextOccurrenceDate] AND ([EndDate] IS NULL OR ([EndDate] >= [StartDate] AND [NextOccurrenceDate] <= [EndDate]))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoalContributions_ContributionAmount_Positive",
                table: "GoalContributions",
                sql: "[ContributionAmount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoalContributionReleases_ReleaseAmount_Positive",
                table: "GoalContributionReleases",
                sql: "[ReleaseAmount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FinancialGoals_TargetAmount_Positive",
                table: "FinancialGoals",
                sql: "[TargetAmount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Budgets_Amount_Positive",
                table: "Budgets",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Budgets_DateRange_Valid",
                table: "Budgets",
                sql: "[StartDate] < [EndDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Balance_NonNegative",
                table: "Accounts",
                sql: "[Balance] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_CreditLimit_Positive",
                table: "Accounts",
                sql: "[CreditLimit] IS NULL OR [CreditLimit] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Currency_USD",
                table: "Accounts",
                sql: "[Currency] = 'USD'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_OpeningBalance_NonNegative",
                table: "Accounts",
                sql: "[OpeningBalance] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Transactions_AccountCombination",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Transactions_Amount_Positive",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Transactions_DifferentAccounts",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_AccountCombination",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Amount_Positive",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_DifferentAccounts",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_ValidDates",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoalContributions_ContributionAmount_Positive",
                table: "GoalContributions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoalContributionReleases_ReleaseAmount_Positive",
                table: "GoalContributionReleases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FinancialGoals_TargetAmount_Positive",
                table: "FinancialGoals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Budgets_Amount_Positive",
                table: "Budgets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Budgets_DateRange_Valid",
                table: "Budgets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_Balance_NonNegative",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_CreditLimit_Positive",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_Currency_USD",
                table: "Accounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Accounts_OpeningBalance_NonNegative",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionType",
                table: "Transactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionType",
                table: "RecurringTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "Frequency",
                table: "RecurringTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "GoalContributionReleases",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
