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

        if (fromDate.HasValue)
            query = query.Where(t => t.TransactionDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.TransactionDate <= toDate.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (transactionType.HasValue)
            query = query.Where(t => t.TransactionType == transactionType.Value);

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
