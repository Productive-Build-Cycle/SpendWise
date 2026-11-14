using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Accounts.GetById;

public sealed record GetAccountByIdQuery(Guid AccountId) : IRequest<AccountDetailsDto>;