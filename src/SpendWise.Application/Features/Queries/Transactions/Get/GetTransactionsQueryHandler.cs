using MediatR;
using SpendWise.Application.Common.Models;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Queries.Transactions.Get;

public sealed class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PagedResult<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUser _currentUser;

    public GetTransactionsQueryHandler(ITransactionRepository transactionRepository, ICurrentUser currentUser)
    {
        _transactionRepository = transactionRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<TransactionDto>> Handle(GetTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        TransactionTypes? transactionType = string.IsNullOrWhiteSpace(request.Type)
            ? null
            : Enum.Parse<TransactionTypes>(request.Type.Trim(), ignoreCase: true);

        var (transactions, totalCount) = await _transactionRepository.ListByUserIdAsync(
            _currentUser.UserId,
            request.PageNumber,
            request.PageSize,
            ToUtc(request.FromDate),
            ToUtc(request.ToDate),
            request.CategoryId,
            transactionType,
            cancellationToken);

        var items = transactions.Select(transaction => new TransactionDto(
            transaction.Id,
            transaction.TransactionType.ToString(),
            transaction.SourceAccountId,
            transaction.DestinationAccountId,
            transaction.CategoryId,
            transaction.Amount.Amount,
            transaction.Amount.Currency.Code,
            transaction.Description,
            transaction.TransactionDate,
            transaction.CreatedAtUtc)).ToList();

        return new PagedResult<TransactionDto>(items, request.PageNumber, request.PageSize, totalCount);
    }

    // Npgsql only accepts UTC values for "timestamp with time zone" columns.
    private static DateTime? ToUtc(DateTime? date)
    {
        if (!date.HasValue)
            return null;

        return date.Value.Kind switch
        {
            DateTimeKind.Utc => date.Value,
            DateTimeKind.Local => date.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
        };
    }
}
