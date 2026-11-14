using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Accounts.GetBalance;

public sealed record GetAccountBalanceQuery(Guid AccountId) : IRequest<AccountBalanceDto>;