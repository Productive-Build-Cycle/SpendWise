using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.Accounts.Get;

public sealed class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, IReadOnlyList<AccountListItemDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUser _currentUser;

    public GetAccountsQueryHandler(IAccountRepository accountRepository, ICurrentUser currentUser)
    {
        _accountRepository = accountRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<AccountListItemDto>> Handle(GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.ListByUserIdAsync(_currentUser.UserId, cancellationToken);

        return accounts.Select(account => new AccountListItemDto(
            account.Id,
            account.AccountName,
            account.AccountType,
            account.Balance.Amount,
            account.Balance.Currency.Code,
            account.CreditLimit?.Amount,
            account.CreatedAtUtc
        )).ToList();
    }
}