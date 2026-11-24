

namespace SpendWise.Application.DTOs
{
    public sealed record GoalContributionDto(
        Guid Id,
        Guid FinancialGoalId,
        decimal Amount,
        string Currency,
        DateOnly ContributionDate,
        Guid AccountId,
        Guid? TransactionId,
        decimal RemainingAmount,
        IReadOnlyList<GoalContributionReleaseDto> Release
        );
}
