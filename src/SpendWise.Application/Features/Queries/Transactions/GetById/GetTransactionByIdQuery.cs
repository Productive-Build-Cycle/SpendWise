using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Transactions.GetById;

public sealed record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionDto>;
