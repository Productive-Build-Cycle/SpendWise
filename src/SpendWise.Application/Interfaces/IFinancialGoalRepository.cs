using SpendWise.Domain.Entities;

namespace SpendWise.Application.Interfaces
{
    public interface IFinancialGoalRepository
    {
        ////used when goal may be modified
        Task<FinancialGoal?> FindByIdForUserAsync(
            Guid financialGoalId,
            Guid userId,
            CancellationToken cancellationToken = default);
        void Add(FinancialGoal financialGoal);

        //used for Get By Id
        Task<FinancialGoal?> FindByIdForUserReadOnlyAsync(
            Guid financialGoalId,
            Guid userId,
            CancellationToken cancellationToken = default);

        //used for Get All Active & Archive
        Task<IReadOnlyList<FinancialGoal>> GetAllForUserReadOnlyAsync(Guid userId,
            CancellationToken cancellationToken = default);


    }
}
