using Newtonsoft.Json;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;
using System.Security.Principal;
using System.Transactions;

namespace SpendWise.Domain.Tests
{
    public class RecurringTransactionTest
    {
        [Fact]
        public void  StartDateInThePast_ThrowsValidationException()
        {
            //Arrange
            var money = new Money(50, new Currency("USD"));
            var startDate = DateTime.UtcNow.AddDays(-10);
            var nextOccurrenceDate = DateTime.UtcNow.AddMonths(1);
            var endDate = DateTime.UtcNow.AddYears(1);
            //Act
            var exception= Assert.Throws<DomainValidationException>(() => new RecurringTransaction(
                                                     Guid.NewGuid(),
                                                     "Internet Bill",
                                                     money,
                                                     Guid.NewGuid(),   // SourceAccountId
                                                     null,             // DestinationAccountId
                                                     TransactionTypes.Expense,
                                                     null,             // CategoryId
                                                     startDate,
                                                     nextOccurrenceDate,
                                                     endDate,
                                                     RecurringFrequency.Monthly
                                                     ));
            //Assert
            Assert.Equal("Start date cannot be in the past.", exception.Message);
        }
        [Fact]
        public void Constructor_NextOccurrenceBeforeStartDate_ThrowValidationException()
        {
            //Arrange
            var money = new Money(50, new Currency("USD"));
            var startDate = DateTime.UtcNow;
            var nextOccurrenceDate = DateTime.UtcNow.AddMonths(-1);
            //Act
            var exception = Assert.Throws<DomainValidationException>(() => new RecurringTransaction(
                                                                 Guid.NewGuid(),
                                                                 "Internet Bill",
                                                                  money,
                                                                  Guid.NewGuid(),
                                                                  null,
                                                                  TransactionTypes.Expense,
                                                                  null,
                                                                  startDate,
                                                                  nextOccurrenceDate,
                                                                  null,
                                                                  RecurringFrequency.Monthly));
            //Assert
            Assert.Equal("Next occurrence date cannot be before the start date.", exception.Message);

        }
        [Fact]
        public void Constructor_EndDateBeforStartDate_ThrowsValidationException()
        {
            //Arrange
            var money = new Money(50, new Currency("USD"));
            var startDate = DateTime.UtcNow;
            var nextOccurrenceDate = DateTime.UtcNow.AddMonths(1);
            var endDate = DateTime.UtcNow.AddDays(-1);
            //Act
            var exception = Assert.Throws<DomainValidationException>(() => new RecurringTransaction(
                                                             Guid.NewGuid(),
                                                             "Internet Bill",
                                                             money,
                                                             Guid.NewGuid(),
                                                             null,
                                                             TransactionTypes.Expense,
                                                             null,
                                                             startDate,
                                                             nextOccurrenceDate,
                                                             endDate,
                                                             RecurringFrequency.Monthly));
            //Assert
            Assert.Equal("End date cannot be before the start date.", exception.Message);
        }
        [Fact]
        public void IncomeTransaction_NullDestinationAccount_ThrowsValidationException()
        {
            //Arrange
            var money = new Money(5000, new Currency("USD"));
            //Act
            var exception =Assert.Throws<DomainValidationException>(()=> new RecurringTransaction(
                             Guid.NewGuid(),
                             "Salary",
                              money,
                              Guid.NewGuid(),//SourceAccountId
                              null,  //DestinatinAccountId
                              TransactionTypes.Income,
                              null,//CategorryId
                              DateTime.UtcNow,
                              DateTime.UtcNow.AddMonths(1),
                              null,
                              RecurringFrequency.Monthly));
            //Assert
            Assert.Equal("Income transactions require a destination account and no source account.", exception.Message);
        }
        [Fact]
        public void IncomeTransaction_NullSourceAndValidDestinaation_CreateSuccessfully()
        {
            //Arrange
            var money = new Money(5000, new Currency("USD"));
            var destinationAccountId = Guid.NewGuid();
            //Act
            var recurringTransaction = new RecurringTransaction(
                                         Guid.NewGuid(),
                                         "Salary",
                                         money,
                                         null,
                                         destinationAccountId,
                                         TransactionTypes.Income,
                                         null,
                                         DateTime.UtcNow,
                                         DateTime.UtcNow.AddMonths(1),
                                         null,
                                         RecurringFrequency.Monthly
                                         );
            //Assert
            Assert.Null(recurringTransaction.SourceAccountId);
            Assert.Equal(recurringTransaction.DestinationAccountId, destinationAccountId);

        }

    }
}
