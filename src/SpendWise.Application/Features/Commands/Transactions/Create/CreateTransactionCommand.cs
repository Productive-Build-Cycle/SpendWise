using MediatR;

namespace SpendWise.Application.Features.Commands.Transactions.Create;

public sealed record CreateTransactionCommand(
    string TransactionType,
    Guid? SourceAccountId,
    Guid? DestinationAccountId,
    Guid? CategoryId,
    decimal Amount,
    string? Description,
    DateTime? TransactionDate
) : IRequest<Guid>;
