using SpendWise.Domain.Entities;

namespace SpendWise.Application.Interfaces
{
    public interface IGoalContributionRepository
    {
        void Add(GoalContribution contribution);
        Task<IReadOnlyList<GoalContribution>> FindByFinancialGoalIdAsync(
            Guid financialGoalId,
              Guid userId,
            CancellationToken cancellationToken = default);
        Task<GoalContribution?> FindByIdForUserAsync(
            Guid goalContributionId,
              Guid userId,
            CancellationToken cancellationToken = default);

    }
}
