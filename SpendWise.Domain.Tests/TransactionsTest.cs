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
                                                               dateTime));

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
                                                               dateTime));
        }

        [Fact]
        public void SameSourceAccountIdAndDestinationAccountId_ThrowsValidationException()
        {
            //Arrange
            var accountId=Guid.NewGuid();
            var transactionType = TransactionTypes.Transfer;
            var amount=new Money(100,new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(()=> new Transactions(accountId,
                                                                     accountId,
                                                                     transactionType,
                                                                     Guid.NewGuid(),
                                                                     amount,
                                                                    "Test transaction",
                                                                     DateTime.UtcNow));
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
                                                                     DateTime.UtcNow));
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
                                                                     DateTime.UtcNow));
        }

        [Fact]
        public void DescriptionExceedsMaxLength_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Transfer;
            var money=new Money(500, new Currency("USD"));
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => new Transactions(Guid.NewGuid(),
                                                                     Guid.NewGuid(),
                                                                     transactionType,
                                                                     Guid.NewGuid(),
                                                                     money,
                                                                    new string('a', 501),
                                                                     DateTime.UtcNow));
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
                                                       DateTime.UtcNow));
        }
        [Fact]
        public void Expense_WithNullSourceAccountId_ThrowsValidationException()
        {
            //Arrange
            var transactionType = TransactionTypes.Expense;
            var money= new Money(500, new Currency("USD"));
            //Act& Asseert
            Assert.Throws<DomainValidationException>(() => new Transactions(
                                                     null,
                                                     Guid.NewGuid(),
                                                     transactionType,
                                                     Guid.NewGuid(),
                                                     money,
                                                       "test transaction",
                                                       DateTime.UtcNow
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
                                                      DateTime.UtcNow));
        }
    }
}
