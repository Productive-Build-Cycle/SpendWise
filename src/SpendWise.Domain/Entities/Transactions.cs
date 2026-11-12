using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;


namespace SpendWise.Domain.Entities
{
    public class Transactions : BaseEntity
    {
        public Guid? SourceAccountId { get; private set; }
        public Account? SourceAccount { get; private set; }

        public Guid? DestinationAccountId { get; private set; }
        public Account? DestinationAccount { get; private set; }

        public TransactionTypes TransactionType { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public Money Amount { get; private set; } = null!;
        public string? Description { get; private set; }

        // The date and time when the transaction occurred.It is not the date and time when the transaction was created in the system.
        public DateTime TransactionDate { get; private set; }


        private Transactions()
        {
        }

        public Transactions(Guid? sourceAccountId,
            Guid? destinationAccountId,
            TransactionTypes transactionType,
            Guid userId,
            Money amount,
            string? description,
            DateTime transactionDate)
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

            if (sourceAccountId.HasValue && destinationAccountId.HasValue &&
                sourceAccountId.Value == destinationAccountId.Value)
            {
                throw new DomainValidationException("Source and destination account IDs cannot be the same.");
            }

            if (sourceAccountId is null && destinationAccountId is null)
            {
                throw new DomainValidationException(
                    "At least one account must be provided.");
            }

            ArgumentNullException.ThrowIfNull(amount, nameof(amount));
            if (amount.Amount <= 0)
            {
                throw new DomainValidationException(
                    "Amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                description = null;
            }
            else
            {
                description = description.Trim();
                if (description.Length > 500)
                {
                    throw new DomainValidationException(
                        "Description cannot exceed 500 characters.");
                }
            }

            if (transactionDate.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Transaction date must be UTC.", nameof(transactionDate));
            }

            if (transactionDate > DateTime.UtcNow)
            {
                throw new DomainValidationException(
                    "Transaction date cannot be in the future.");
            }

            switch (transactionType)
            {
                case TransactionTypes.Income:
                    if (sourceAccountId.HasValue || !destinationAccountId.HasValue)
                    {
                        throw new DomainValidationException(
                            "Income transactions require a destination account and no source account.");
                    }

                    break;

                case TransactionTypes.Expense:
                    if (!sourceAccountId.HasValue || destinationAccountId.HasValue)
                    {
                        throw new DomainValidationException(
                            "Expense transactions require a source account and no destination account.");
                    }

                    break;

                case TransactionTypes.Transfer:
                    if (!sourceAccountId.HasValue || !destinationAccountId.HasValue)
                    {
                        throw new DomainValidationException(
                            "Transfer transactions require both source and destination accounts.");
                    }

                    break;

                default:
                    throw new DomainValidationException(
                        "Invalid transaction type.");
            }

            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
            TransactionType = transactionType;
            UserId = userId;
            Amount = amount;
            Description = description;
            TransactionDate = transactionDate;
        }
    }
}