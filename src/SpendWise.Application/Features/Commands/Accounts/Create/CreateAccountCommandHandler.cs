using MediatR;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Application.Features.Commands.Accounts.Create;

public sealed class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private const string SupportedCurrency = "USD";

    private readonly IAccountRepository _accountRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public CreateAccountCommandHandler(IAccountRepository accountRepository, IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _accountRepository = accountRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var currency = new Currency(SupportedCurrency);

        var openingBalance = new Money(request.OpeningBalance, currency);

        Money? creditLimit = request.CreditLimit.HasValue ? new Money(request.CreditLimit.Value, currency) : null;

        var typeName = Enum.GetNames<AccountType>()
            .FirstOrDefault(name => string.Equals(name, request.AccountType.Trim(), StringComparison.OrdinalIgnoreCase));

        if (typeName is null)
            throw new ValidationException("Invalid account type.");

        var accountType = Enum.Parse<AccountType>(typeName);
        
        var account = new Account(
            _currentUser.UserId,
            request.AccountName,
            accountType,
            openingBalance,
            creditLimit);

        _accountRepository.Add(account);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}