using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.Enums;

namespace SpendWise.Domain.Tests
{
    public class TransactionsTest
    {
        [Fact]
        public void Constructor_EmptyUserId_ThrowsArgumentException()
        {
            //Arrange
            var amount = new Money(100, new Currency("USD"));
            var description = "Test transaction";
            var dateTime = DateTime.UtcNow;
            //Act & Assert
            Assert.Throws<ArgumentException>(() => new Transactions(null,
                Guid.NewGuid(),
                TransactionTypes.Income,
                Guid.Empty,
                amount,
                description,
                dateTime,
                Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_NullSourceAccountIdAndNullDestinationAccountId_ThrowsValidationException()
        {
            //Arrange
            var amount = new Money(100, new Currency("USD"));
            var description = "Test transaction";
            var dateTime = DateTime.UtcNow;
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(null,
                null,
                TransactionTypes.Income,
                Guid.NewGuid(),
                amount,
                description,
                dateTime,
                Guid.NewGuid()));
        }

        [Fact]
        public void SameSourceAccountIdAndDestinationAccountId_ThrowsValidationException()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var transactionType = TransactionTypes.Transfer;
            var amount = new Money(100, new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(accountId,
                accountId,
                transactionType,
                Guid.NewGuid(),
                amount,
                "Test transaction",
                DateTime.UtcNow
                , null));
        }

        [Fact]
        public void NullSourceAccountIdAndNullDestinationAccountId_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Transfer;
            var amount = new Money(100, new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(null,
                null,
                transactionType,
                Guid.NewGuid(),
                amount,
                "Test transaction",
                DateTime.UtcNow,
                null));
        }

        [Fact]
        public void AmountLessThanOrEqualToZero_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Income;
            var amount = new Money(0, new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(Guid.NewGuid(),
                Guid.NewGuid(),
                transactionType,
                Guid.NewGuid(),
                amount,
                "Test transaction",
                DateTime.UtcNow,
                Guid.NewGuid()));
        }

        [Fact]
        public void DescriptionExceedsMaxLength_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Transfer;
            var money = new Money(500, new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(Guid.NewGuid(),
                Guid.NewGuid(),
                transactionType,
                Guid.NewGuid(),
                money,
                new string('a', 501),
                DateTime.UtcNow,
                null));
        }

        [Fact]
        public void Income_WithDestinationAccountIdNull_ThrowsValidationException()
        {
            //Arrange
            var transaction = TransactionTypes.Income;
            var money = new Money(500, new Currency("USD"));
            //Act&Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(
                Guid.NewGuid(),
                null,
                transaction,
                Guid.NewGuid(),
                money,
                "test transaction",
                DateTime.UtcNow,
                Guid.NewGuid()));
        }

        [Fact]
        public void Expense_WithNullSourceAccountId_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Expense;
            var money = new Money(500, new Currency("USD"));
            //Act& Asseert
            Assert.Throws<DomainValidationException>(() => new Transactions(
                null,
                Guid.NewGuid(),
                transactionType,
                Guid.NewGuid(),
                money,
                "test transaction",
                DateTime.UtcNow,
                Guid.NewGuid()
            ));
        }

        [Fact]
        public void Transfer_WithNullSourceAccountId_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Transfer;
            var money = new Money(500, new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(
                null,
                Guid.NewGuid(),
                transactionType,
                Guid.NewGuid(),
                money,
                "test transaction",
                DateTime.UtcNow,
                null));
        }

        [Fact]
        public void Income_WithoutCategory_ThrowsValidationException()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Transactions(
                    null,
                    Guid.NewGuid(),
                    TransactionTypes.Income,
                    Guid.NewGuid(),
                    new Money(100, new Currency("USD")),
                    "Salary",
                    DateTime.UtcNow,
                    null));
        }

        [Fact]
        public void Expense_WithoutCategory_ThrowsValidationException()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Transactions(
                    Guid.NewGuid(),
                    null,
                    TransactionTypes.Expense,
                    Guid.NewGuid(),
                    new Money(100, new Currency("USD")),
                    "Food",
                    DateTime.UtcNow,
                    null));
        }

        [Fact]
        public void Transfer_WithCategory_ThrowsValidationException()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Transactions(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    TransactionTypes.Transfer,
                    Guid.NewGuid(),
                    new Money(100, new Currency("USD")),
                    "Transfer",
                    DateTime.UtcNow,
                    Guid.NewGuid()));
        }

        [Fact]
        public void Income_WithCategory_CreatesSuccessfully()
        {
            var categoryId = Guid.NewGuid();

            var transaction = new Transactions(
                null,
                Guid.NewGuid(),
                TransactionTypes.Income,
                Guid.NewGuid(),
                new Money(1000, new Currency("USD")),
                "Salary",
                DateTime.UtcNow,
                categoryId);

            Assert.Equal(categoryId, transaction.CategoryId);
        }

        [Fact]
        public void Expense_WithCategory_CreatesSuccessfully()
        {
            var categoryId = Guid.NewGuid();

            var transaction = new Transactions(
                Guid.NewGuid(),
                null,
                TransactionTypes.Expense,
                Guid.NewGuid(),
                new Money(100, new Currency("USD")),
                "Food",
                DateTime.UtcNow,
                categoryId);

            Assert.Equal(categoryId, transaction.CategoryId);
        }

        [Fact]
        public void Constructor_EmptyCategoryId_ThrowsValidationException()
        {
            Assert.Throws<DomainValidationException>(() =>
                new Transactions(
                    null,
                    Guid.NewGuid(),
                    TransactionTypes.Income,
                    Guid.NewGuid(),
                    new Money(100, new Currency("USD")),
                    "Salary",
                    DateTime.UtcNow,
                    Guid.Empty));
        }
    }
}