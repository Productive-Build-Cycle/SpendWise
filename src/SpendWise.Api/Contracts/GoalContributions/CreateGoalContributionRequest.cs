namespace SpendWise.Api.Contracts.GoalContributions
{
    public sealed record CreateGoalContributionRequest(
        Guid FinancialGoalId,
        decimal Amount,
        DateTimeOffset ContributionDate,
        Guid AccountId,
        Guid? SourceAccountId
        );

}
