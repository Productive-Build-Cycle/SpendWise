using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Commands.GoalContributionRelease.Create
{
    public sealed record CreateGoalContributionReleaseCommand(
        Guid GoalContributionId,
        decimal ReleaseAmount,
        DateTimeOffset ReleaseDate
        ) : IRequest<GoalContributionReleaseDto>;

}
