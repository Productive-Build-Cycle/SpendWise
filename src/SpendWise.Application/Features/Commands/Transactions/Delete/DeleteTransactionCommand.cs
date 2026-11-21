using MediatR;

namespace SpendWise.Application.Features.Commands.Transactions.Delete;

public sealed record DeleteTransactionCommand(Guid TransactionId) : IRequest<Unit>;