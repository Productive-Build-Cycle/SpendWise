using FluentValidation.Validators;
using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.FinancialGoals.GetAll
{
    public sealed class GetAllFinancialGoalsQueryHandler :
        IRequestHandler<GetAllFinancialGoalsQuery, IReadOnlyList<FinancialGoalDto>>
    {
        private readonly IFinancialGoalRepository _financialGoalRepository;
        private readonly ICurrentUser _currentUser;

        public GetAllFinancialGoalsQueryHandler(
            IFinancialGoalRepository financialGoalRepository,
            ICurrentUser currentUser)
        {
            _financialGoalRepository = financialGoalRepository;
            _currentUser = currentUser;
        }
        public async Task<IReadOnlyList<FinancialGoalDto>> Handle(
            GetAllFinancialGoalsQuery request,
            CancellationToken cancellationToken)
        {
            var goals = await _financialGoalRepository.GetAllForUserReadOnlyAsync(
                _currentUser.UserId,
                 cancellationToken);

            return goals.Select(g =>
            {
                var totalSaved = g.Contributions.Sum(
                    contribution => contribution.RemainingAmount.Amount);

                var remainigAmount = Math.Max(
                    0,
                    g.TargetAmount.Amount - totalSaved);
                var progressPercentage = g.TargetAmount.Amount > 0
                ? Math.Round(
                    totalSaved / g.TargetAmount.Amount * 100m, 2)
                : 0;

                return new FinancialGoalDto(
                    g.Id,
                    g.GoalName,
                    g.TargetAmount.Amount,
                    g.TargetAmount.Currency.Code,
                    g.TargetDate,
                    totalSaved,
                    remainigAmount,
                    progressPercentage,
                    g.IsArchived);
            }).ToList();
        }
    }
}
