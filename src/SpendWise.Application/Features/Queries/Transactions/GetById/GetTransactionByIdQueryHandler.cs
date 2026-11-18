using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.Transactions.GetById;

public sealed class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUser _currentUser;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, ICurrentUser currentUser)
    {
        _transactionRepository = transactionRepository;
        _currentUser = currentUser;
    }

    public async Task<TransactionDto> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction =
            await _transactionRepository.FindByIdForUserReadOnlyAsync(request.TransactionId, _currentUser.UserId,
                cancellationToken);

        if (transaction is null)
            throw new NotFoundException("Transaction not found.");

        return new TransactionDto(
            transaction.Id,
            transaction.TransactionType.ToString(),
            transaction.SourceAccountId,
            transaction.DestinationAccountId,
            transaction.CategoryId,
            transaction.Amount.Amount,
            transaction.Amount.Currency.Code,
            transaction.Description,
            transaction.TransactionDate,
            transaction.CreatedAtUtc);
    }
}
