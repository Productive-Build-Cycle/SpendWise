using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.FinancialGoals.GetById
{
    public sealed class GetFinancialGoalByIdQueryHandler :
        IRequestHandler<GetFinancialGoalByIdQuery, FinancialGoalDto>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IFinancialGoalRepository _financialGoalRepository;

        public GetFinancialGoalByIdQueryHandler(ICurrentUser currentUser,
            IFinancialGoalRepository financialRepository)
        {
            _currentUser = currentUser;
            _financialGoalRepository = financialRepository;
        }
        public async Task<FinancialGoalDto> Handle(GetFinancialGoalByIdQuery request,
            CancellationToken cancellationToken = default)
        {

            var goal = await _financialGoalRepository.FindByIdForUserReadOnlyAsync(
                request.GoalId, _currentUser.UserId, cancellationToken);
            if (goal == null)
            {
                throw new NotFoundException("Financial goal not found !");
            }
            var totalSaved = goal.Contributions.Sum(
                contribution => contribution.RemainingAmount.Amount);
            var remainingAmount = Math.Max(0,
                goal.TargetAmount.Amount - totalSaved);
            var progressPercentage = goal.TargetAmount.Amount > 0
                ? Math.Round(
                    totalSaved / goal.TargetAmount.Amount * 100m, 2)
                : 0;

            return new FinancialGoalDto(
                goal.UserId,
                goal.GoalName,
                goal.TargetAmount.Amount,
                goal.TargetAmount.Currency.Code,
                goal.TargetDate,
                totalSaved,
                remainingAmount,
                progressPercentage,
                goal.IsArchived);
        }
    }
}
