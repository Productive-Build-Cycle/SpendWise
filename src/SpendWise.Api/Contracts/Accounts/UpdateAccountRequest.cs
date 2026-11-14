namespace SpendWise.Api.Contracts.Accounts;

public sealed record UpdateAccountRequest(
    string AccountName,
    decimal? CreditLimit);