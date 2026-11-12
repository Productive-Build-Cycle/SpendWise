using MediatR;

namespace SpendWise.Application.Features.Commands.Accounts.Create;

public sealed record CreateAccountCommand(
    string AccountName,
    string AccountType,
    decimal OpeningBalance,
    decimal? CreditLimit
) : IRequest<Guid>;