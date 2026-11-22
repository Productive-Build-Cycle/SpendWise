using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.Queries.GoalContributions.GetByFinancialGoalId
{
    public sealed record GetGoalContributionByGoalIdQueryHandler :
        IRequestHandler<GetGoalContributionByGoalIdQuery, IReadOnlyList<GoalContributionDto>>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IFinancialGoalRepository _financialGoalRepository;
        private readonly IGoalContributionRepository _goalContributionRepository;

        public GetGoalContributionByGoalIdQueryHandler(
            ICurrentUser currentUser,
            IFinancialGoalRepository financialGoalRepository,
            IGoalContributionRepository goalContributionRepository)
        {
            _currentUser = currentUser;
            _financialGoalRepository = financialGoalRepository;
            _goalContributionRepository = goalContributionRepository;
        }
        public async Task<IReadOnlyList<GoalContributionDto>> Handle(
            GetGoalContributionByGoalIdQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            var financialGoal = await _financialGoalRepository.FindByIdForUserReadOnlyAsync(
                request.FinancialGoalId,
                userId,
                cancellationToken);
            if (financialGoal is null)
            {
                throw new NotFoundException("Financial goal was not found");
            }

            var contributions = await _goalContributionRepository.FindByFinancialGoalIdAsync
                (financialGoal.Id,
                userId,
                cancellationToken);

            return contributions.Select(contribution => new GoalContributionDto(
                contribution.Id,
                financialGoal.Id,
                contribution.ContributionAmount.Amount,
                contribution.ContributionAmount.Currency.Code,
                contribution.ContributionDate,
                contribution.AccountId,
                contribution.TransactionId,
                contribution.RemainingAmount.Amount,
                contribution.Releases.Select(r => new GoalContributionReleaseDto(
                    r.Id,
                    contribution.FinancialGoalId,
                    financialGoal.GoalName,
                    contribution.Id,
                    r.ReleaseAmount.Amount,
                    r.ReleaseDate,
                    contribution.AccountId,
                    r.TransactionId
                    )).ToList()
                )).ToList();
        }
    }

}
