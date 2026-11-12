using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;



namespace SpendWise.Domain.Entities
{
    public class RecurringTransaction:BaseEntity
    {
        public Guid UserId { get;private set; }
        public User User { get; private set; } = null!;
        public string TransactionName { get; private set; } = string.Empty;
        public Money Amount { get; private set; }
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
            Amount= null!;
        }
        public RecurringTransaction(Guid userId,
                                    string transactionName,
                                    Money amount,
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
                throw new ArgumentException(
                    "User ID cannot be empty.",
                    nameof(userId));
            }
          
            ValidateSchedule(
                   transactionName,
                   amount,
                   sourceAccountId,
                   destinationAccountId,
                   transactionType,
                   categoryId,
                   startDate,
                   nextOccurrenceDate,
                   endDate,
                   frequency);
            
            if (startDate.Date < DateTime.UtcNow.Date)
            {
                throw new DomainValidationException("Start date cannot be in the past.");
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

        private static void ValidateSchedule(
                        string transactionName,
                        Money amount,
                        Guid? sourceAccountId,
                        Guid? destinationAccountId,
                        TransactionTypes transactionType,
                        Guid? categoryId,
                        DateTime startDate,
                        DateTime nextOccurrenceDate,
                        DateTime? endDate,
                        RecurringFrequency frequency)
        {
            if (startDate.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Start date must be UTC.", nameof(startDate));
            }

            if (nextOccurrenceDate.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Next occurrence date must be UTC.", nameof(nextOccurrenceDate));
            }

            if (endDate.HasValue && endDate.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("End date must be UTC.", nameof(endDate));
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
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Category ID cannot be empty.", nameof(categoryId));
            }
            if (string.IsNullOrWhiteSpace(transactionName))
            {
                throw new ArgumentException("Transaction name cannot be null or empty.", nameof(transactionName));
            }
            transactionName = transactionName.Trim();
            if(transactionName.Length > 100)
            {
                throw new DomainValidationException(
                    "Transaction name cannot exceed 100 characters.");
            }
            ArgumentNullException.ThrowIfNull(amount, nameof(amount));
            if (amount.Amount <= 0)
            {
                throw new DomainValidationException(
                   "Amount must be greater than zero.");
            }
            if (!Enum.IsDefined(typeof(TransactionTypes), transactionType))
            {
                throw new DomainValidationException(
                    "Invalid transaction type.");
            }
            if (!Enum.IsDefined(typeof(RecurringFrequency), frequency))
            {
                throw new DomainValidationException(
                    "Invalid recurring frequency.");
            }
            if (startDate > nextOccurrenceDate)
            {
                throw new DomainValidationException(
                    "Next occurrence date cannot be before the start date.");
            }

            if (endDate.HasValue && endDate.Value < startDate)
            {
                throw new DomainValidationException(
                    "End date cannot be before the start date.");
            }

            if (endDate.HasValue && nextOccurrenceDate > endDate.Value)
            {
                throw new DomainValidationException(
                     "Next occurrence date cannot be after the end date.");
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
                    throw new DomainValidationException("Invalid transaction type.");
            }
        }
       
        public DateTime? CalculateNextOccurrence()
        {
           DateTime nextDate= Frequency switch
            {
                RecurringFrequency.Daily => NextOccurrenceDate.AddDays(1),
                RecurringFrequency.Weekly => NextOccurrenceDate.AddDays(7),
                RecurringFrequency.Monthly => NextOccurrenceDate.AddMonths(1),
                RecurringFrequency.Yearly => NextOccurrenceDate.AddYears(1),
                _ => throw new DomainValidationException("Invalid recurring frequency.")
            };
            if(EndDate.HasValue && nextDate > EndDate.Value)
            {
                return null;
            }
            return nextDate;
        }
        public void UpdateNextOccurrenceDate()
        {
           
            DateTime? nextDate = CalculateNextOccurrence();
            if (!nextDate.HasValue)
            {
                throw new DomainValidationException(
                    "The recurring transaction schedule has ended.");
            }

            NextOccurrenceDate = nextDate.Value;
        }
        public void UpdateSchedule(
            string transactionName,
            Guid? sourceAccountId,
            Guid? destinationAccountId,
            TransactionTypes transactionType,
            Guid? categoryId,
            Money amount,
            DateTime? endDate,
            RecurringFrequency frequency
            )
        {
            ValidateSchedule(
                    transactionName,
                    amount,
                    sourceAccountId,
                    destinationAccountId,
                    transactionType,
                    categoryId,
                    StartDate,
                    NextOccurrenceDate,
                    endDate,
                    frequency);
            TransactionName = transactionName.Trim(); 
            Amount = amount;
            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
            TransactionType = transactionType;
            CategoryId = categoryId;
            EndDate = endDate;
            Frequency = frequency;

        }

        
    }
}
