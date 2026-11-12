namespace SpendWise.Api.Contracts.Accounts;

public sealed record CreateAccountRequest(
    string AccountName,
    string AccountType,
    decimal OpeningBalance,
    decimal? CreditLimit);