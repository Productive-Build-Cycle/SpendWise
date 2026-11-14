using SpendWise.Domain.Entities;

namespace SpendWise.Application.Interfaces;

public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Account?> FindByIdForUserAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);

    Task<Account?> FindByIdForUserReadOnlyAsync(Guid accountId, Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> HasReferencesAsync(Guid accountId, CancellationToken cancellationToken = default);

    void Add(Account account);

    void Remove(Account account);
}