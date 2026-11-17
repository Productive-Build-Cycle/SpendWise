using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Tests
{
    public class TransactionsApplyToAccountsTests
    {
        private static readonly Currency Usd = new("USD");

        private static Account CreateAccount(Guid userId, decimal balance,
            AccountType accountType = AccountType.Chequing, decimal? creditLimit = null)
        {
            return new Account(
                userId,
                "Test account",
                accountType,
                new Money(balance, Usd),
                creditLimit.HasValue ? new Money(creditLimit.Value, Usd) : null);
        }

        private static Transactions CreateTransaction(TransactionTypes type, Guid userId, decimal amount,
            Account? source, Account? destination)
        {
            return new Transactions(
                source?.Id,
                destination?.Id,
                type,
                userId,
                new Money(amount, Usd),
                "Test transaction",
                DateTime.UtcNow,
                type == TransactionTypes.Transfer ? null : Guid.NewGuid());
        }

        [Fact]
        public void Income_IncreasesDestinationBalance()
        {
            var userId = Guid.NewGuid();
            var destination = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Income, userId, 50, null, destination);

            transaction.ApplyToAccounts(null, destination);

            Assert.Equal(150, destination.Balance.Amount);
        }

        [Fact]
        public void Expense_DecreasesSourceBalance()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 40, source, null);

            transaction.ApplyToAccounts(source, null);

            Assert.Equal(60, source.Balance.Amount);
        }

        [Fact]
        public void Transfer_MovesMoneyBetweenAccounts()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var destination = CreateAccount(userId, 10, AccountType.Savings);
            var transaction = CreateTransaction(TransactionTypes.Transfer, userId, 30, source, destination);

            transaction.ApplyToAccounts(source, destination);

            Assert.Equal(70, source.Balance.Amount);
            Assert.Equal(40, destination.Balance.Amount);
        }

        [Fact]
        public void Expense_InsufficientFunds_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 20);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 50, source, null);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(source, null));
            Assert.Equal(20, source.Balance.Amount);
        }

        [Fact]
        public void Transfer_InsufficientFunds_DoesNotChangeDestinationBalance()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 20);
            var destination = CreateAccount(userId, 10);
            var transaction = CreateTransaction(TransactionTypes.Transfer, userId, 50, source, destination);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(source, destination));
            Assert.Equal(10, destination.Balance.Amount);
        }

        [Fact]
        public void Expense_FromCreditCard_IncreasesOutstandingDebt()
        {
            var userId = Guid.NewGuid();
            var card = CreateAccount(userId, 100, AccountType.CreditCard, 1000);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 200, card, null);

            transaction.ApplyToAccounts(card, null);

            Assert.Equal(300, card.Balance.Amount);
        }

        [Fact]
        public void Expense_FromCreditCard_ExceedingLimit_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var card = CreateAccount(userId, 900, AccountType.CreditCard, 1000);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 200, card, null);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(card, null));
            Assert.Equal(900, card.Balance.Amount);
        }

        [Fact]
        public void Transfer_ToCreditCard_DecreasesOutstandingDebt()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 500);
            var card = CreateAccount(userId, 300, AccountType.CreditCard, 1000);
            var transaction = CreateTransaction(TransactionTypes.Transfer, userId, 100, source, card);

            transaction.ApplyToAccounts(source, card);

            Assert.Equal(400, source.Balance.Amount);
            Assert.Equal(200, card.Balance.Amount);
        }

        [Fact]
        public void ApplyToAccounts_MissingSourceAccount_ThrowsArgumentNullException()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 10, source, null);

            Assert.Throws<ArgumentNullException>(() => transaction.ApplyToAccounts(null, null));
        }

        [Fact]
        public void ApplyToAccounts_UnexpectedDestinationAccount_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var other = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 10, source, null);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(source, other));
        }

        [Fact]
        public void ApplyToAccounts_AccountDoesNotMatchTransaction_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var otherAccount = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 10, source, null);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(otherAccount, null));
        }

        [Fact]
        public void ApplyToAccounts_AccountBelongsToAnotherUser_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var foreignAccount = CreateAccount(Guid.NewGuid(), 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 10, foreignAccount, null);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(foreignAccount, null));
            Assert.Equal(100, foreignAccount.Balance.Amount);
        }

        [Fact]
        public void Income_ToLoanAccount_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var loan = CreateAccount(userId, 100, AccountType.Loan);
            var transaction = CreateTransaction(TransactionTypes.Income, userId, 10, null, loan);

            Assert.Throws<DomainValidationException>(() => transaction.ApplyToAccounts(null, loan));
        }
    }
}
