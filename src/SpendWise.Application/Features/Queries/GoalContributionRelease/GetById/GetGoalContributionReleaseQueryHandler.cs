using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Queries.GoalContributionRelease.GetById
{
    public sealed class GetGoalContributionReleaseQueryHandler :
        IRequestHandler<GetGoalContributionReleaseByIdQuery,
            GoalContributionReleaseDto>
    {
        private readonly IGoalContributionReleaseRepository _releaseRepository;
        private readonly ICurrentUser _currentUser;

        public GetGoalContributionReleaseQueryHandler(
            IGoalContributionReleaseRepository releaseRepository,
            ICurrentUser currentUser
            )
        {
            _releaseRepository = releaseRepository;
            _currentUser = currentUser;
        }
        public async Task<GoalContributionReleaseDto> Handle(
            GetGoalContributionReleaseByIdQuery request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId;
            var release = await _releaseRepository.FindByIdForUserReadOnlyAsync(
                request.releaseId,
                userId,
                cancellationToken);
            if (release is null)
            {
                throw new NotFoundException("Release not found .");
            }
            var goalContribution = release.GoalContribution;
            var financialGoal = goalContribution.FinancialGoal;
            return new GoalContributionReleaseDto(
                release.Id,
                financialGoal.Id,
                financialGoal.GoalName,
                goalContribution.Id,
                release.ReleaseAmount.Amount,
                release.ReleaseDate,
                goalContribution.AccountId,
                release.TransactionId
                );
        }
    }
}
