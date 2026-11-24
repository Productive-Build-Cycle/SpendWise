using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Common.Models;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Queries.Transactions.Get;

public sealed class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, PagedResult<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public GetTransactionsQueryHandler(
        ITransactionRepository transactionRepository,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _transactionRepository = transactionRepository;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<TransactionDto>> Handle(GetTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = await _userRepository.GetUserByIdAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User was not found.");
        }

        TransactionTypes? transactionType = string.IsNullOrWhiteSpace(request.Type)
            ? null
            : Enum.Parse<TransactionTypes>(request.Type.Trim(), ignoreCase: true);

        var (transactions, totalCount) = await _transactionRepository.ListByUserIdAsync(
            _currentUser.UserId,
            request.PageNumber,
            request.PageSize,
            ToUtc(request.FromDate, user.TimeZoneId),
            ToUtc(request.ToDate, user.TimeZoneId),
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
    private static DateTime? ToUtc(DateTime? date, string timeZoneId)
    {
        if (!date.HasValue)
            return null;

        /* return date.Value.Kind switch
         {
             DateTimeKind.Utc => date.Value,
             DateTimeKind.Local => date.Value.ToUniversalTime(),  //uses the server's local time zone, not the user's time zone.
             _ => DateTime.SpecifyKind(date.Value, DateTimeKind.Utc) //only labels  as UTC
         }; */
        if (date.Value.Kind == DateTimeKind.Utc)
            return date.Value;
        TimeZoneInfo userTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime localDateTime = DateTime.SpecifyKind(
            date.Value,
            DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(
            localDateTime,
            userTimeZone);
    }
}
