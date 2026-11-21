using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Interfaces;

public interface ITransactionRepository
{
    Task<(IReadOnlyList<Transactions> Items, int TotalCount)> ListByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? categoryId,
        TransactionTypes? transactionType,
        CancellationToken cancellationToken = default);

    Task<Transactions?> FindByIdForUserReadOnlyAsync(Guid transactionId, Guid userId,
        CancellationToken cancellationToken = default);

    Task<Transactions?> FindByIdForUserAsync(Guid transactionId, Guid userId,
        CancellationToken cancellationToken = default);

    void Add(Transactions transaction);

    void Remove(Transactions transaction);
}
