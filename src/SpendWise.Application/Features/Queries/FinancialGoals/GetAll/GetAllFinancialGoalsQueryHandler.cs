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
        public async Task<IReadOnlyList<FinancialGoalDto>>Handle (
            GetAllFinancialGoalsQuery request ,
            CancellationToken cancellationToken)
        {
            var goals = await _financialGoalRepository.GetAllForUserReadOnlyAsync(
                _currentUser.UserId,
                 cancellationToken);

            return goals.Select(g => new FinancialGoalDto(
               g.Id,
               g.GoalName,
               g.TargetAmount.Amount,
               g.TargetAmount.Currency.Code,
               g.TargetDate,
               g.IsArchived
                )).ToList();
        }
    }
}
