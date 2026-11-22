using SpendWise.Domain.Entities;

namespace SpendWise.Application.Interfaces
{
    public interface IGoalContributionReleaseRepository
    {
        void Add(GoalContributionRelease goalContributionReleasId);

        Task<IReadOnlyList<GoalContributionRelease>> FindByGoalContributionIdForUserAsync(
            Guid GoalContributionId,
            Guid userId,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<GoalContributionRelease>> FindByGoalContributionIdForUserReadOnlyAsync(
            Guid GoalContributionId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<GoalContributionRelease?> FindByIdForUserReadOnlyAsync(
            Guid Id,
            Guid UserId,
            CancellationToken cancellationToken = default);
    }
}
