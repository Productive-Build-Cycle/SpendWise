namespace SpendWise.Api.Contracts.FinancialGoals
{
    public sealed record UpdateFinancialGoalRequest(
        string GoalName,
        decimal TargetAmount,
        string CurrencyCode,
        DateOnly TargetDate);

}
