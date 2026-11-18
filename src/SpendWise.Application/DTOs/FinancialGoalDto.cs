namespace SpendWise.Application.DTOs
{
    public sealed record FinancialGoalDto(
        Guid Id,

        string GoalName,

        decimal TargetAmount,

        string CurrencyCode,

        DateOnly TargetDate,

        bool IsArchived);
}
