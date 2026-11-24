using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Repositories
{
    public class GoalContributionRepository : IGoalContributionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GoalContributionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(GoalContribution contribution)
        {
            _dbContext.GoalContributions.Add(contribution);
        }

        public async Task<GoalContribution?> FindByIdForUserAsync(
            Guid goalContributionId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.GoalContributions
                .Include(c => c.FinancialGoal)
                .Include(c => c.Releases)
                .SingleOrDefaultAsync(
                    g => g.Id == goalContributionId &&
                         g.FinancialGoal.UserId == userId,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<GoalContribution>> FindByFinancialGoalIdAsync(
            Guid financialGoalId,
              Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.GoalContributions
                .Include(g => g.Releases)
                 .Where(gc => gc.FinancialGoalId == financialGoalId &&
                 gc.FinancialGoal.UserId == userId)
                 .ToListAsync(cancellationToken);
        }
    }
}
