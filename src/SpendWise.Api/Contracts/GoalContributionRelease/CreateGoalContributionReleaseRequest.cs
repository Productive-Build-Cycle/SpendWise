using MediatR;

namespace SpendWise.Api.Contracts.GoalContributionRelease
{
    public sealed record CreateGoalContributionReleaseRequest(
        Guid GoalContributionId,
        decimal ReleaseAmount,
        DateTimeOffset ReleaseDate
        );

}
