using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.Accounts.GetById;

public sealed class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDetailsDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUser _currentUser;

    public GetAccountByIdQueryHandler(IAccountRepository accountRepository, ICurrentUser currentUser)
    {
        _accountRepository = accountRepository;
        _currentUser = currentUser;
    }

    public async Task<AccountDetailsDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account =
            await _accountRepository.FindByIdForUserReadOnlyAsync(request.AccountId, _currentUser.UserId,
                cancellationToken);

        if (account is null)
            throw new NotFoundException("Account not found.");

        return new AccountDetailsDto(
            account.Id,
            account.AccountName,
            account.AccountType.ToString(),
            account.OpeningBalance.Amount,
            account.Balance.Amount,
            account.Balance.Currency.Code,
            account.CreditLimit?.Amount,
            account.CreatedAtUtc,
            account.UpdatedAtUtc);
    }
}