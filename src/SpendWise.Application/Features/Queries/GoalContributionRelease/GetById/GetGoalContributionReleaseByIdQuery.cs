using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.GoalContributionRelease.GetById
{
    public sealed record GetGoalContributionReleaseByIdQuery(Guid releaseId) :
        IRequest<GoalContributionReleaseDto>;

}
