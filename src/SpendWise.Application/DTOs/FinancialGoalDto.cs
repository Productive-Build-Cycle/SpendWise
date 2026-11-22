namespace SpendWise.Application.DTOs
{
    public sealed record FinancialGoalDto(
        Guid Id,

        string GoalName,

        decimal TargetAmount,

        string CurrencyCode,

        DateOnly TargetDate,

        decimal TotalSaved,

        decimal RemainingAmount,

        decimal ProgressPercetage,

        bool IsArchived);
}
