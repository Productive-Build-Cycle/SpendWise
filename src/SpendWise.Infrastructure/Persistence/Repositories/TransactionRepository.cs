using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Transactions> Items, int TotalCount)> ListByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? categoryId,
        TransactionTypes? transactionType,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        // Pattern matching copies each value into a non-nullable local, so the lambdas never read a nullable.
        if (fromDate is { } fromValue)
            query = query.Where(t => t.TransactionDate >= fromValue);

        if (toDate is { } toValue)
            query = query.Where(t => t.TransactionDate <= toValue);

        if (categoryId is { } categoryValue)
            query = query.Where(t => t.CategoryId == categoryValue);

        if (transactionType is { } typeValue)
            query = query.Where(t => t.TransactionType == typeValue);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAtUtc)
            .ThenBy(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Transactions?> FindByIdForUserReadOnlyAsync(Guid transactionId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Transactions
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId, cancellationToken);
    }

    public void Add(Transactions transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        _dbContext.Transactions.Add(transaction);
    }
}
