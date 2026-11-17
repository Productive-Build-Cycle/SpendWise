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

        public Guid? CategoryId { get; private set; }
        public Category? Category { get; private set; }

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

        public Transactions(Guid? sourceAccountId, Guid? destinationAccountId, TransactionTypes transactionType,
            Guid userId, Money amount, string? description, DateTime transactionDate, Guid? categoryId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (sourceAccountId == Guid.Empty)
                throw new ArgumentException("Source account ID cannot be empty.", nameof(sourceAccountId));

            if (destinationAccountId == Guid.Empty)
                throw new ArgumentException("Destination account ID cannot be empty.", nameof(destinationAccountId));

            if (sourceAccountId.HasValue && destinationAccountId.HasValue &&
                sourceAccountId.Value == destinationAccountId.Value)
                throw new DomainValidationException("Source and destination account IDs cannot be the same.");

            if (sourceAccountId is null && destinationAccountId is null)
                throw new DomainValidationException("At least one account must be provided.");

            ArgumentNullException.ThrowIfNull(amount, nameof(amount));

            if (amount.Amount <= 0)
                throw new DomainValidationException("Amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(description))
                description = null;
            else
            {
                description = description.Trim();
                if (description.Length > 500)
                    throw new DomainValidationException("Description cannot exceed 500 characters.");
            }

            if (transactionDate.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Transaction date must be UTC.", nameof(transactionDate));

            if (transactionDate > DateTime.UtcNow)
                throw new DomainValidationException("Transaction date cannot be in the future.");

            if (categoryId == Guid.Empty)
                throw new DomainValidationException("Category ID cannot be empty.");

            switch (transactionType)
            {
                case TransactionTypes.Income:
                    if (sourceAccountId.HasValue || !destinationAccountId.HasValue)
                        throw new DomainValidationException(
                            "Income transactions require a destination account and no source account.");
                    if (!categoryId.HasValue)
                        throw new DomainValidationException("Income transactions require a category.");
                    break;

                case TransactionTypes.Expense:
                    if (!sourceAccountId.HasValue || destinationAccountId.HasValue)
                        throw new DomainValidationException(
                            "Expense transactions require a source account and no destination account.");
                    if (!categoryId.HasValue)
                        throw new DomainValidationException("Expense transactions require a category.");
                    break;

                case TransactionTypes.Transfer:
                    if (!sourceAccountId.HasValue || !destinationAccountId.HasValue)
                        throw new DomainValidationException(
                            "Transfer transactions require both source and destination accounts.");
                    if (categoryId.HasValue)
                        throw new DomainValidationException("Transfer transactions cannot have a category.");

                    break;

                default:
                    throw new DomainValidationException("Invalid transaction type.");
            }

            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
            TransactionType = transactionType;
            UserId = userId;
            Amount = amount;
            Description = description;
            TransactionDate = transactionDate;
            CategoryId = categoryId;
        }

        /// <summary>
        /// Applies this transaction to the given accounts so that balances stay consistent with it.
        /// Income increases the destination balance, expense decreases the source balance,
        /// and a transfer does both.
        /// For credit cards the balance is the outstanding debt, so a debit is a purchase
        /// and a credit is a payment.
        /// </summary>
        public void ApplyToAccounts(Account? sourceAccount, Account? destinationAccount)
        {
            if (SourceAccountId.HasValue)
            {
                ArgumentNullException.ThrowIfNull(sourceAccount, nameof(sourceAccount));
                EnsureAccountMatches(sourceAccount, SourceAccountId.Value, "Source");
            }
            else if (sourceAccount is not null)
            {
                throw new DomainValidationException("This transaction does not have a source account.");
            }

            if (DestinationAccountId.HasValue)
            {
                ArgumentNullException.ThrowIfNull(destinationAccount, nameof(destinationAccount));
                EnsureAccountMatches(destinationAccount, DestinationAccountId.Value, "Destination");
            }
            else if (destinationAccount is not null)
            {
                throw new DomainValidationException("This transaction does not have a destination account.");
            }

            // The debit runs first so a failed withdrawal never leaves a partially applied transfer.
            if (sourceAccount is not null)
                Debit(sourceAccount, Amount);

            if (destinationAccount is not null)
                Credit(destinationAccount, Amount);
        }

        private void EnsureAccountMatches(Account account, Guid expectedAccountId, string role)
        {
            if (account.Id != expectedAccountId)
                throw new DomainValidationException($"{role} account does not match the transaction.");

            if (account.UserId != UserId)
                throw new DomainValidationException($"{role} account must belong to the transaction's user.");

            if (account.Balance.Currency != Amount.Currency)
                throw new DomainValidationException($"{role} account currency must match the transaction currency.");
        }

        private static void Debit(Account account, Money amount)
        {
            if (account.AccountType == AccountType.CreditCard)
                account.MakeCreditCardPurchase(amount);
            else
                account.Withdraw(amount);
        }

        private static void Credit(Account account, Money amount)
        {
            if (account.AccountType == AccountType.CreditCard)
                account.PayCreditCard(amount);
            else
                account.Deposit(amount);
        }
    }
}