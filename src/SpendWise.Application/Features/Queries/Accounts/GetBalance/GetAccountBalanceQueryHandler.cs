using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Queries.Accounts.GetBalance;

public sealed class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, AccountBalanceDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUser _currentUser;

    public GetAccountBalanceQueryHandler(IAccountRepository accountRepository, ICurrentUser currentUser)
    {
        _accountRepository = accountRepository;
        _currentUser = currentUser;
    }

    public async Task<AccountBalanceDto> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
    {
        var account =
            await _accountRepository.FindByIdForUserReadOnlyAsync(request.AccountId, _currentUser.UserId,
                cancellationToken);

        if (account is null)
            throw new NotFoundException("Account not found.");

        decimal? availableCredit = account.AccountType == AccountType.CreditCard
            ? account.CreditLimit!.Amount - account.Balance.Amount
            : null;

        return new AccountBalanceDto(
            account.Id,
            account.AccountType.ToString(),
            account.Balance.Amount,
            account.Balance.Currency.Code,
            account.CreditLimit?.Amount,
            availableCredit);
    }
}