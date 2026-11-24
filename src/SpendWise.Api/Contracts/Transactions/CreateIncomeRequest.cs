namespace SpendWise.Api.Contracts.Transactions;

public sealed record CreateIncomeRequest(
    Guid DestinationAccountId,
    Guid CategoryId,
    decimal Amount,
    string? Description,
    DateTimeOffset? TransactionDate);
