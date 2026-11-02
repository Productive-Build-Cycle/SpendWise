using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoneyValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Transactions_Amount_Positive",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Amount_Positive",
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

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "Amount_Amount");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "RecurringTransactions",
                newName: "Amount_Amount");

            migrationBuilder.RenameColumn(
                name: "ContributionAmount",
                table: "GoalContributions",
                newName: "ContributionAmount_Amount");

            migrationBuilder.RenameColumn(
                name: "ReleaseAmount",
                table: "GoalContributionReleases",
                newName: "ReleaseAmount_Amount");

            migrationBuilder.RenameColumn(
                name: "TargetAmount",
                table: "FinancialGoals",
                newName: "TargetAmount_Amount");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Budgets",
                newName: "Amount_Amount");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Accounts",
                newName: "OpeningBalance_Currency");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance",
                table: "Accounts",
                newName: "OpeningBalance_Amount");

            migrationBuilder.RenameColumn(
                name: "CreditLimit",
                table: "Accounts",
                newName: "CreditLimit_Amount");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Accounts",
                newName: "Balance_Amount");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "Transactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "RecurringTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "ContributionAmount_Currency",
                table: "GoalContributions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "ReleaseAmount_Currency",
                table: "GoalContributionReleases",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "TargetAmount_Currency",
                table: "FinancialGoals",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "Budgets",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "Balance_Currency",
                table: "Accounts",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "CreditLimit_Currency",
                table: "Accounts",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);
            migrationBuilder.Sql(
                "UPDATE Accounts "+
                "Set CreditLimit_Currency = 'USD' " +
                "Where CreditLimit_Amount IS NOT NULL");
                

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transactions_Amount_Positive",
                table: "Transactions",
                sql: "[Amount_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Amount_Positive",
                table: "RecurringTransactions",
                sql: "[Amount_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoalContributions_ContributionAmount_Positive",
                table: "GoalContributions",
                sql: "[ContributionAmount_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoalContributions_Currency_USD",
                table: "GoalContributions",
                sql: "[ContributionAmount_Currency] = 'USD'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoalContributionReleases_Currency_USD",
                table: "GoalContributionReleases",
                sql: "[ReleaseAmount_Currency] = 'USD'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoalContributionReleases_ReleaseAmount_Positive",
                table: "GoalContributionReleases",
                sql: "[ReleaseAmount_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FinancialGoals_Currency_USD",
                table: "FinancialGoals",
                sql: "[TargetAmount_Currency] = 'USD'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FinancialGoals_TargetAmount_Positive",
                table: "FinancialGoals",
                sql: "[TargetAmount_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Budgets_Amount_Positive",
                table: "Budgets",
                sql: "[Amount_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Balance_NonNegative",
                table: "Accounts",
                sql: "[Balance_Amount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_CreditLimit_Positive",
                table: "Accounts",
                sql: "[CreditLimit_Amount] IS NULL OR [CreditLimit_Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_Currency_USD",
                table: "Accounts",
                sql: "[OpeningBalance_Currency] = 'USD' And [Balance_Currency] = 'USD' And ([CreditLimit_Amount] Is Null Or [CreditLimit_Currency] = 'USD')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Accounts_OpeningBalance_NonNegative",
                table: "Accounts",
                sql: "[OpeningBalance_Amount] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Transactions_Amount_Positive",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecurringTransactions_Amount_Positive",
                table: "RecurringTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoalContributions_ContributionAmount_Positive",
                table: "GoalContributions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoalContributions_Currency_USD",
                table: "GoalContributions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoalContributionReleases_Currency_USD",
                table: "GoalContributionReleases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoalContributionReleases_ReleaseAmount_Positive",
                table: "GoalContributionReleases");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FinancialGoals_Currency_USD",
                table: "FinancialGoals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FinancialGoals_TargetAmount_Positive",
                table: "FinancialGoals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Budgets_Amount_Positive",
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
                name: "Amount_Currency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "RecurringTransactions");

            migrationBuilder.DropColumn(
                name: "ContributionAmount_Currency",
                table: "GoalContributions");

            migrationBuilder.DropColumn(
                name: "ReleaseAmount_Currency",
                table: "GoalContributionReleases");

            migrationBuilder.DropColumn(
                name: "TargetAmount_Currency",
                table: "FinancialGoals");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Balance_Currency",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreditLimit_Currency",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "Transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "RecurringTransactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "ContributionAmount_Amount",
                table: "GoalContributions",
                newName: "ContributionAmount");

            migrationBuilder.RenameColumn(
                name: "ReleaseAmount_Amount",
                table: "GoalContributionReleases",
                newName: "ReleaseAmount");

            migrationBuilder.RenameColumn(
                name: "TargetAmount_Amount",
                table: "FinancialGoals",
                newName: "TargetAmount");

            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "Budgets",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance_Currency",
                table: "Accounts",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance_Amount",
                table: "Accounts",
                newName: "OpeningBalance");

            migrationBuilder.RenameColumn(
                name: "CreditLimit_Amount",
                table: "Accounts",
                newName: "CreditLimit");

            migrationBuilder.RenameColumn(
                name: "Balance_Amount",
                table: "Accounts",
                newName: "Balance");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transactions_Amount_Positive",
                table: "Transactions",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecurringTransactions_Amount_Positive",
                table: "RecurringTransactions",
                sql: "[Amount] > 0");

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
    }
}
