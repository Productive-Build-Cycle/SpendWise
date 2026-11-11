using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Domain.Tests
{
    public class AccountTests
    {
        [Fact]
        public void Constructor_ValidAccount_CreatesAccountSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accountName = "My Bank Account";
            var accountType = AccountType.Chequing;
            var openingBalance = new Money(1000, new Currency("USD"));
            // Act
            var account = new Account(userId, accountName, accountType, openingBalance);
            // Assert
            Assert.Equal(userId, account.UserId);
            Assert.Equal(accountName, account.AccountName);
            Assert.Equal(accountType, account.AccountType);
            Assert.Equal(openingBalance.Amount, account.OpeningBalance.Amount);
            Assert.Equal(openingBalance.Currency.Code, account.OpeningBalance.Currency.Code);
        }

        [Fact]
        public void Deposit_ValidDeposit_IncreasesBalance()
        {
            // Arrange
            var account = new Account(
                Guid.NewGuid(),
                "My Savings",
                AccountType.Savings,
                new Money(1000, new Currency("USD"))
            );
            var deposit = new Money(500, new Currency("USD"));
            // Act
            account.Deposit(deposit);

            // Assert
            Assert.Equal(1500, account.Balance.Amount);
        }

        [Fact]
        public void Withdraw_ValidWithdrawal_DecreasesBalance()
        {
            // Arrange
            var account = new Account(
                Guid.NewGuid(),
                "My Savings",
                AccountType.Savings,
                new Money(1000, new Currency("USD"))
            );
            var withdrawal = new Money(200, new Currency("USD"));
            // Act
            account.Withdraw(withdrawal);
            // Assert
            Assert.Equal(800, account.Balance.Amount);
        }

        [Fact]
        public void PayCreditCard_ValidPayment_DecreasesOutstandingBalance()
        {
            // Arrange
            var account = new Account(
                Guid.NewGuid(),
                "My Credit Card",
                AccountType.CreditCard,
                new Money(1000, new Currency("USD")),
                new Money(5000, new Currency("USD"))
            );
            var payment = new Money(300, new Currency("USD"));
            // Act
            account.PayCreditCard(payment);

            // Assert
            Assert.Equal(1000, account.OpeningBalance.Amount);
            Assert.Equal(700, account.Balance.Amount);
        }

        [Fact]
        public void MakeCreditCardPurchase_PurchaseExceedsCreditLimit_ThrowsValidationException()
        {
            // Arrange
            var account = new Account(
                Guid.NewGuid(),
                "My Credit Card",
                AccountType.CreditCard,
                new Money(1000, new Currency("USD")),
                new Money(5000, new Currency("USD"))
            );
            var purchase = new Money(4500, new Currency("USD"));
            // Act & Assert
            Assert.Throws<ValidationException>(() => account.MakeCreditCardPurchase(purchase));
        }

        [Fact]
        public void Constructor_NonCreditCardWithCreditLimit_ThrowsValidationException()
        {
            var openingBalance = new Money(1000, new Currency("USD"));
            var creditLimit = new Money(5000, new Currency("USD"));

            Assert.Throws<ValidationException>(() =>
                new Account(
                    Guid.NewGuid(),
                    "My Savings",
                    AccountType.Savings,
                    openingBalance,
                    creditLimit));
        }

        [Fact]
        public void Constructor_CreditCardWithoutCreditLimit_ThrowsValidationException()
        {
            var openingBalance = new Money(1000, new Currency("USD"));

            Assert.Throws<ValidationException>(() =>
                new Account(
                    Guid.NewGuid(),
                    "My Credit Card",
                    AccountType.CreditCard,
                    openingBalance));
        }

        [Fact]
        public void Constructor_OpeningBalanceExceedsCreditLimit_ThrowsValidationException()
        {
            var openingBalance = new Money(6000, new Currency("USD"));
            var creditLimit = new Money(5000, new Currency("USD"));

            Assert.Throws<ValidationException>(() =>
                new Account(
                    Guid.NewGuid(),
                    "My Credit Card",
                    AccountType.CreditCard,
                    openingBalance,
                    creditLimit));
        }

        [Fact]
        public void Withdraw_AmountExceedsBalance_ThrowsValidationException()
        {
            var account = new Account(
                Guid.NewGuid(),
                "My Savings",
                AccountType.Savings,
                new Money(1000, new Currency("USD")));

            var withdrawal = new Money(1001, new Currency("USD"));

            Assert.Throws<ValidationException>(() =>
                account.Withdraw(withdrawal));

            Assert.Equal(1000m, account.Balance.Amount);
        }
    }
}