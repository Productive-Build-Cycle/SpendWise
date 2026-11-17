using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;


namespace SpendWise.Infrastructure.Persistence.Repositories
{
    public class FinancialGoalRepository : IFinancialGoalRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public FinancialGoalRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<FinancialGoal?> FindByIdForUserAsync(Guid financialGoalId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.FinancialGoals.SingleOrDefaultAsync(
                 g => g.UserId == userId && g.Id == financialGoalId,
                 cancellationToken);
        }
        
        public void Add(FinancialGoal financialGoal)
        {
            _dbContext.FinancialGoals.Add(financialGoal);
        }
    }
}
