namespace SpendWise.Api.Contracts.Transactions;

public sealed record CreateExpenseRequest(
    Guid SourceAccountId,
    Guid CategoryId,
    decimal Amount,
    string? Description,
    DateTime? TransactionDate);
