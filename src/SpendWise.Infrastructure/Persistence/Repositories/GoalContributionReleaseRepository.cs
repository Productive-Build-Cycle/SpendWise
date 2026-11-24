using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Repositories
{
    public class GoalContributionReleaseRepository : IGoalContributionReleaseRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GoalContributionReleaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(GoalContributionRelease goalContributionReleas)
        {
            _dbContext.GoalContributionReleases.Add(goalContributionReleas);
        }

        public async Task<IReadOnlyList<GoalContributionRelease>>
             FindByGoalContributionIdForUserAsync(
             Guid goalContributionId,
             Guid userId,
             CancellationToken cancellationToken = default)
        {
            return await _dbContext.GoalContributionReleases
                 .Where(gcr => gcr.GoalContributionId == goalContributionId &&
                 gcr.GoalContribution.FinancialGoal.UserId == userId)
                 .ToListAsync(cancellationToken);

        }
        public async Task<IReadOnlyList<GoalContributionRelease>> FindByGoalContributionIdForUserReadOnlyAsync(
            Guid goalContributionId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.GoalContributionReleases
                .AsNoTracking()
                .Where(gcr => gcr.GoalContributionId == goalContributionId &&
                gcr.GoalContribution.FinancialGoal.UserId == userId)
                .ToListAsync(cancellationToken);

        }
        public async Task<GoalContributionRelease?> FindByIdForUserReadOnlyAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.GoalContributionReleases
                .AsNoTracking()
                .Include(r => r.GoalContribution)
                .ThenInclude(g => g.FinancialGoal)
                .SingleOrDefaultAsync(r => r.Id == id &&
                r.GoalContribution.FinancialGoal.UserId == userId,
                cancellationToken);
        }
    }
}
