using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;

namespace SpendWise.Infrastructure.Persistence;

public class EfTransactionManager : ITransactionManager
{
    private readonly ApplicationDbContext _dbContext;

    public EfTransactionManager(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _dbContext.ChangeTracker.Clear();

            throw new ConcurrencyConflictException("A concurrent database update was detected.", ex);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}