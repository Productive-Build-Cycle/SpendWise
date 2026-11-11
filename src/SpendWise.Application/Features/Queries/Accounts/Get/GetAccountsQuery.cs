using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Accounts.Get;

public sealed record GetAccountsQuery : IRequest<IReadOnlyList<AccountListItemDto>>;