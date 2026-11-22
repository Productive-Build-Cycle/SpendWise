using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Commands.GoalContributions.Create
{
    public sealed record CreateGoalContributionCommand(
        Guid FinancialGoalId,
        Guid AccountId,   //Account where the goal contribbution is held
        Guid? SourceAccountId, //The account funding the contribution,null means money is already in goal account so no transfer is neede
        decimal Amount,
        DateTimeOffset ContributionDate) : IRequest<GoalContributionDto>;

}
