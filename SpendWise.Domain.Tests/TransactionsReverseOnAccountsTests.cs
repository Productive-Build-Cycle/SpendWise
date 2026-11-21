using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Tests
{
    public class TransactionsReverseOnAccountsTests
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
        public void Income_Reverse_RestoresDestinationBalance()
        {
            var userId = Guid.NewGuid();
            var destination = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Income, userId, 50, null, destination);
            transaction.ApplyToAccounts(null, destination);

            transaction.ReverseOnAccounts(null, destination);

            Assert.Equal(100, destination.Balance.Amount);
        }

        [Fact]
        public void Expense_Reverse_RestoresSourceBalance()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 40, source, null);
            transaction.ApplyToAccounts(source, null);

            transaction.ReverseOnAccounts(source, null);

            Assert.Equal(100, source.Balance.Amount);
        }

        [Fact]
        public void Transfer_Reverse_RestoresBothBalances()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var destination = CreateAccount(userId, 10, AccountType.Savings);
            var transaction = CreateTransaction(TransactionTypes.Transfer, userId, 30, source, destination);
            transaction.ApplyToAccounts(source, destination);

            transaction.ReverseOnAccounts(source, destination);

            Assert.Equal(100, source.Balance.Amount);
            Assert.Equal(10, destination.Balance.Amount);
        }

        [Fact]
        public void Income_Reverse_WhenMoneyWasSpent_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var destination = CreateAccount(userId, 100);
            var income = CreateTransaction(TransactionTypes.Income, userId, 80, null, destination);
            income.ApplyToAccounts(null, destination);
            var expense = CreateTransaction(TransactionTypes.Expense, userId, 150, destination, null);
            expense.ApplyToAccounts(destination, null);

            Assert.Throws<DomainValidationException>(() => income.ReverseOnAccounts(null, destination));
            Assert.Equal(30, destination.Balance.Amount);
        }

        [Fact]
        public void Transfer_Reverse_WhenDestinationWasSpent_DoesNotChangeSourceBalance()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var destination = CreateAccount(userId, 0, AccountType.Savings);
            var transfer = CreateTransaction(TransactionTypes.Transfer, userId, 60, source, destination);
            transfer.ApplyToAccounts(source, destination);
            var expense = CreateTransaction(TransactionTypes.Expense, userId, 50, destination, null);
            expense.ApplyToAccounts(destination, null);

            Assert.Throws<DomainValidationException>(() => transfer.ReverseOnAccounts(source, destination));
            Assert.Equal(40, source.Balance.Amount);
            Assert.Equal(10, destination.Balance.Amount);
        }

        [Fact]
        public void Expense_OnCreditCard_Reverse_DecreasesOutstandingDebt()
        {
            var userId = Guid.NewGuid();
            var card = CreateAccount(userId, 100, AccountType.CreditCard, 1000);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 200, card, null);
            transaction.ApplyToAccounts(card, null);

            transaction.ReverseOnAccounts(card, null);

            Assert.Equal(100, card.Balance.Amount);
        }

        [Fact]
        public void Expense_OnCreditCard_Reverse_WhenDebtWasAlreadyPaid_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var card = CreateAccount(userId, 0, AccountType.CreditCard, 1000);
            var chequing = CreateAccount(userId, 500);
            var purchase = CreateTransaction(TransactionTypes.Expense, userId, 200, card, null);
            purchase.ApplyToAccounts(card, null);
            var payment = CreateTransaction(TransactionTypes.Transfer, userId, 200, chequing, card);
            payment.ApplyToAccounts(chequing, card);

            Assert.Throws<DomainValidationException>(() => purchase.ReverseOnAccounts(card, null));
            Assert.Equal(0, card.Balance.Amount);
        }

        [Fact]
        public void Transfer_ToCreditCard_Reverse_RestoresOutstandingDebt()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 500);
            var card = CreateAccount(userId, 300, AccountType.CreditCard, 1000);
            var transaction = CreateTransaction(TransactionTypes.Transfer, userId, 100, source, card);
            transaction.ApplyToAccounts(source, card);

            transaction.ReverseOnAccounts(source, card);

            Assert.Equal(500, source.Balance.Amount);
            Assert.Equal(300, card.Balance.Amount);
        }

        [Fact]
        public void ReverseOnAccounts_MissingSourceAccount_ThrowsArgumentNullException()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 10, source, null);

            Assert.Throws<ArgumentNullException>(() => transaction.ReverseOnAccounts(null, null));
        }

        [Fact]
        public void ReverseOnAccounts_AccountDoesNotMatchTransaction_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var source = CreateAccount(userId, 100);
            var otherAccount = CreateAccount(userId, 100);
            var transaction = CreateTransaction(TransactionTypes.Expense, userId, 10, source, null);

            Assert.Throws<DomainValidationException>(() => transaction.ReverseOnAccounts(otherAccount, null));
            Assert.Equal(100, otherAccount.Balance.Amount);
        }
    }
}
