using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.GoalContributions.GetByFinancialGoalId
{
    public sealed record GetGoalContributionByGoalIdQuery(
       Guid FinancialGoalId) : IRequest<IReadOnlyList<GoalContributionDto>>;


}
