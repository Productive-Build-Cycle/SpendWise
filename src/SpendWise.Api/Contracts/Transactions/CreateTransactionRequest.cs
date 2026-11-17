namespace SpendWise.Api.Contracts.Transactions;

public sealed record CreateTransactionRequest(
    string TransactionType,
    Guid? SourceAccountId,
    Guid? DestinationAccountId,
    Guid? CategoryId,
    decimal Amount,
    string? Description,
    DateTime? TransactionDate);
