using MediatR;

namespace SpendWise.Application.Features.Commands.Accounts.Update;

public sealed record UpdateAccountCommand(
    Guid AccountId,
    string AccountName,
    decimal? CreditLimit
) : IRequest<Unit>;