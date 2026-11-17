using SpendWise.Domain.Entities;

namespace SpendWise.Application.Interfaces
{
    public interface IFinancialGoalRepository
    {
        Task<FinancialGoal?> FindByIdForUserAsync(
            Guid financialGoalId,
            Guid userId,
            CancellationToken cancellationToken = default);
        void Add(FinancialGoal financialGoal);
    }
}
