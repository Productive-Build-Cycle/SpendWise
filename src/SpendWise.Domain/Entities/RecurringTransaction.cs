using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;


namespace SpendWise.Domain.Entities
{
    public class RecurringTransaction:BaseEntity
    {
        public Guid UserId { get;private set; }
        public User User { get; private set; } = null!;
        public string TransactionName { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public Guid? SourceAccountId { get; private set; }
        public Account? SourceAccount { get; private set; } 
        public Guid? DestinationAccountId { get; private set; }
        public Account? DestinationAccount { get; private set; }
        public TransactionTypes TransactionType { get; private set; }
        public Guid? CategoryId { get; private set; } 
        public Category? Category { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime NextOccurrenceDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public RecurringFrequency Frequency { get; private set; } 
        private RecurringTransaction()
        {
        }
        public RecurringTransaction(Guid userId,
                                    string transactionName,
                                    decimal amount,
                                    Guid? sourceAccountId,
                                    Guid? destinationAccountId,
                                    TransactionTypes transactionType,
                                    Guid? categoryId,
                                    DateTime startDate,
                                    DateTime nextOccurrenceDate,
                                    DateTime? endDate,
                                    RecurringFrequency frequency)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (sourceAccountId == Guid.Empty)
            {
                throw new ArgumentException("Source account ID cannot be empty.", nameof(sourceAccountId));
            }
            if (destinationAccountId == Guid.Empty)
            {
                throw new ArgumentException("Destination account ID cannot be empty.", nameof(destinationAccountId));
            }
            if (sourceAccountId.HasValue && destinationAccountId.HasValue && sourceAccountId.Value == destinationAccountId.Value)
            {
                throw new ArgumentException("Source and destination account IDs cannot be the same.");
            }
            if (sourceAccountId is null && destinationAccountId is null)
            {
                throw new ArgumentException(
                    "At least one account must be provided.");
            }
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Category ID cannot be empty.", nameof(categoryId));
            }
            if (string.IsNullOrWhiteSpace(transactionName))
            {
                throw new ArgumentException("Transaction name cannot be null or empty.", nameof(transactionName));
            }
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }
            if (startDate > nextOccurrenceDate)
            {
                throw new ArgumentException("Next occurrence date must be after the start date.", nameof(nextOccurrenceDate));
            }
            if (endDate.HasValue && endDate.Value < startDate)
            {
                throw new ArgumentException("End date must be after the start date.", nameof(endDate));
            }
            if (endDate.HasValue && nextOccurrenceDate > endDate.Value)
            {
                throw new ArgumentException(
                    "Next occurrence date cannot be after the end date.",
                    nameof(nextOccurrenceDate));
            }
            switch (transactionType)
            {
                case TransactionTypes.Income:
                    if (sourceAccountId.HasValue || !destinationAccountId.HasValue)
                    {
                        throw new ArgumentException(
                            "Income transactions require a destination account and no source account.");
                    }
                    break;

                case TransactionTypes.Expense:
                    if (!sourceAccountId.HasValue || destinationAccountId.HasValue)
                    {
                        throw new ArgumentException(
                            "Expense transactions require a source account and no destination account.");
                    }
                    break;

                case TransactionTypes.Transfer:
                    if (!sourceAccountId.HasValue || !destinationAccountId.HasValue)
                    {
                        throw new ArgumentException(
                            "Transfer transactions require both source and destination accounts.");
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(transactionType),
                        "Invalid transaction type.");
            }
            UserId = userId;
            TransactionName = transactionName.Trim();
            Amount = amount;
            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
            TransactionType = transactionType;
            CategoryId = categoryId;
            StartDate = startDate;
            NextOccurrenceDate = nextOccurrenceDate;
            EndDate = endDate;
            Frequency = frequency;
        }

    }
}
