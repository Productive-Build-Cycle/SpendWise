using MediatR;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Commands.Accounts.Create;

public sealed record CreateAccountCommand(
    string AccountName,
    AccountType AccountType,
    decimal OpeningBalance,
    decimal? CreditLimit
) : IRequest<Guid>;