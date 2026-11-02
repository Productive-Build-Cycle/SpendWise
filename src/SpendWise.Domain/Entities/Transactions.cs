using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;


namespace SpendWise.Domain.Entities
{
    public class Transactions: BaseEntity
    {
        public Guid? SourceAccountId { get;private set; }
        public Account? SourceAccount { get; private set; } 

        public Guid? DestinationAccountId { get; private set; }
        public Account? DestinationAccount { get; private set; } 

        public TransactionTypes TransactionType { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public Money Amount { get; private set; } = null!;
        public string? Description { get; private set; } 
        public DateTime TransactionDate { get; private  set; } // The date and time when the transaction occurred.It is not the date and time when the transaction was created in the system.

        
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
            if(sourceAccountId == Guid.Empty )
            {
                throw new ArgumentException("Source account ID cannot be empty.", nameof(sourceAccountId));
            }
            if(destinationAccountId == Guid.Empty )
            {
                throw new ArgumentException("Destination account ID cannot be empty.", nameof(destinationAccountId));
            }
            if (sourceAccountId.HasValue && destinationAccountId.HasValue && 
                sourceAccountId.Value == destinationAccountId.Value)
            {
                throw new ValidationException("Source and destination account IDs cannot be the same.");
            }
            if (sourceAccountId is null && destinationAccountId is null)
            {
                throw new ValidationException(
                    "At least one account must be provided.");
            }
            ArgumentNullException.ThrowIfNull(amount, nameof(amount));
            if (amount.Amount <= 0)
            {
                throw new ValidationException(
                   "Amount must be greater than zero." );
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
                    throw new ValidationException(
                        "Description cannot exceed 500 characters.");
                }
            }
           
            if (transactionDate > DateTime.UtcNow)
            {
                throw new ValidationException(
                    "Transaction date cannot be in the future." );
            }
            switch (transactionType)
            {
                case TransactionTypes.Income:
                    if (sourceAccountId.HasValue || !destinationAccountId.HasValue)
                    {
                        throw new ValidationException(
                            "Income transactions require a destination account and no source account.");
                    }
                    break;

                case TransactionTypes.Expense:
                    if (!sourceAccountId.HasValue || destinationAccountId.HasValue)
                    {
                        throw new ValidationException(
                            "Expense transactions require a source account and no destination account.");
                    }
                    break;

                case TransactionTypes.Transfer:
                    if (!sourceAccountId.HasValue || !destinationAccountId.HasValue)
                    {
                        throw new ValidationException(
                            "Transfer transactions require both source and destination accounts.");
                    }
                    break;

                default:
                    throw new ValidationException(
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
