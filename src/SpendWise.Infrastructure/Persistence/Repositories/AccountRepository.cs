using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AccountRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Account>> ListByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.AccountName)
            .ToListAsync(cancellationToken);
    }

    public Task<Account?> FindByIdForUserAsync(Guid accountId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Accounts
            .SingleOrDefaultAsync(a => a.Id == accountId && a.UserId == userId, cancellationToken);
    }

    public Task<Account?> FindByIdForUserReadOnlyAsync(Guid accountId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == accountId && a.UserId == userId, cancellationToken);
    }

    public async Task<bool> HasReferencesAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Transactions
                .AnyAsync(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId,
                    cancellationToken))
            return true;

        if (await _dbContext.RecurringTransactions
                .AnyAsync(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId,
                    cancellationToken))
            return true;

        return await _dbContext.GoalContributions.AnyAsync(c => c.AccountId == accountId, cancellationToken);
    }

    public void Add(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        _dbContext.Accounts.Add(account);
    }

    public void Remove(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        _dbContext.Accounts.Remove(account);
    }
}