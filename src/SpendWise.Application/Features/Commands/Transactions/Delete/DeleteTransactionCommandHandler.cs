using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Commands.Transactions.Delete;

public sealed class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Unit>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public DeleteTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var transaction = await _transactionRepository.FindByIdForUserAsync(
            request.TransactionId, userId, cancellationToken);

        if (transaction is null)
            throw new NotFoundException("Transaction not found.");

        Account? sourceAccount = null;
        if (transaction.SourceAccountId.HasValue)
        {
            sourceAccount = await _accountRepository.FindByIdForUserAsync(
                transaction.SourceAccountId.Value, userId, cancellationToken);

            if (sourceAccount is null)
                throw new NotFoundException("Source account not found.");
        }

        Account? destinationAccount = null;
        if (transaction.DestinationAccountId.HasValue)
        {
            destinationAccount = await _accountRepository.FindByIdForUserAsync(
                transaction.DestinationAccountId.Value, userId, cancellationToken);

            if (destinationAccount is null)
                throw new NotFoundException("Destination account not found.");
        }

        // Balances are restored together with the removal in a single SaveChanges call.
        transaction.ReverseOnAccounts(sourceAccount, destinationAccount);

        _transactionRepository.Remove(transaction);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}