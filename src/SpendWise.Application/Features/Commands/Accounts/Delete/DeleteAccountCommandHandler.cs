using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Commands.Accounts.Delete;

public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Unit>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public DeleteAccountCommandHandler(
        IAccountRepository accountRepository,
        IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _accountRepository = accountRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.AccountId == Guid.Empty)
            throw new FluentValidation.ValidationException("Account ID is required.");

        var account =
            await _accountRepository.FindByIdForUserAsync(request.AccountId, _currentUser.UserId, cancellationToken);

        if (account is null)
            throw new NotFoundException("Account not found.");

        var hasReferences = await _accountRepository.HasReferencesAsync(account.Id, cancellationToken);

        if (hasReferences)
            throw new ConflictException("Account cannot be deleted because it has financial records.");

        _accountRepository.Remove(account);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}