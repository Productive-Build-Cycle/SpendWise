using SpendWise.Domain.Enums;

namespace SpendWise.Application.DTOs;

public sealed record AccountListItemDto(
    Guid Id,
    string AccountName,
    string AccountType,
    decimal Balance,
    string CurrencyCode,
    decimal? CreditLimit,
    DateTime CreatedAtUtc);
    
public sealed record AccountDetailsDto(
    Guid Id,
    string AccountName,
    string AccountType,
    decimal OpeningBalance,
    decimal Balance,
    string CurrencyCode,
    decimal? CreditLimit,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
    
public sealed record AccountBalanceDto(
    Guid AccountId,
    string AccountType,
    decimal Balance,
    string CurrencyCode,
    decimal? CreditLimit,
    decimal? AvailableCredit);