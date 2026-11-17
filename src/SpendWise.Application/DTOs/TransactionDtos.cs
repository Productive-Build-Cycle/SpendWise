namespace SpendWise.Application.DTOs;

public sealed record TransactionDto(
    Guid Id,
    string TransactionType,
    Guid? SourceAccountId,
    Guid? DestinationAccountId,
    Guid? CategoryId,
    decimal Amount,
    string CurrencyCode,
    string? Description,
    DateTime TransactionDate,
    DateTime CreatedAtUtc);
