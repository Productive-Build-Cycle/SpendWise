using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Application.Features.Commands.Accounts.Update;

public sealed class UpdateAccountCommandHandler
    : IRequestHandler<UpdateAccountCommand, Unit>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public UpdateAccountCommandHandler(
        IAccountRepository accountRepository, 
        IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _accountRepository = accountRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.FindByIdForUserAsync(request.AccountId, _currentUser.UserId, cancellationToken);

        if (account is null)
            throw new NotFoundException("Account not found.");

        account.Rename(request.AccountName);

        if (account.AccountType == AccountType.CreditCard)
        {
            if (!request.CreditLimit.HasValue)
                throw new ValidationException("Credit limit is required for credit card accounts.");

            account.UpdateCreditLimit(new Money(request.CreditLimit.Value, account.Balance.Currency));
        }
        else if (request.CreditLimit.HasValue)
            throw new ValidationException("Credit limit is only allowed for credit card accounts.");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}