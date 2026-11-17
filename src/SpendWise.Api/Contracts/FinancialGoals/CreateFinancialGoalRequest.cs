namespace SpendWise.Api.Contracts.FinancialGoals
{
    public sealed record CreateFinancialGoalRequest(
        string GoalName,
        decimal TargetAmount,
        string CurrencyCode,
        DateOnly TargetDate);
}
