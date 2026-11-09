using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;
using SpendWise.Domain.Entities;

namespace SpendWise.Domain.Tests
{
    public class FinancialGoalTests
    {
        [Fact]
        public void AddContribution_DuplicateContribution_ThrowsValidationException()
        {   // Arrange
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000,new Currency("USD")),
                 DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))

                );
            var contribution = new GoalContribution(
                goal.Id,
                new Money(1000,new Currency("USD")),
                DateOnly.FromDateTime(DateTime.UtcNow),
                Guid.NewGuid()//AccountId
            );
            goal.AddContribution(contribution);
            //Act & Assert
            Assert.Throws<ValidationException>(() => goal.AddContribution(contribution));
        }

        [Fact]
        public void AddContribution_IsArchivedGoal_ThrowsValidationException()
        {
            // Arrange
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                 DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
                );
            var contribution = new GoalContribution(
                goal.Id,
                new Money(1000, new Currency("USD")),
                DateOnly.FromDateTime(DateTime.UtcNow),
                Guid.NewGuid()//AccountId
            );
            var remainingAmount = new Money(0, new Currency("USD"));
            goal.Archive(remainingAmount);
            // Act & Assert
            Assert.Throws<ValidationException>(() => goal.AddContribution(contribution));
        }
        [Fact]
        public void Archive_FinancialGoalWithRemainingAmount_ThrowsValidationException()
        {
            // Arrange
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                 DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
                );
            var remainingAmount = new Money(0, new Currency("USD"));
            goal.Archive(remainingAmount);
            var contribution = new GoalContribution(
                goal.Id,
                new Money(1000, new Currency("USD")),
                DateOnly.FromDateTime(DateTime.UtcNow),
                Guid.NewGuid()//AccountId
            );
            // Act & Assert
            Assert.Throws<ValidationException>(() => goal.AddContribution(contribution));
        }
        [Fact]
        public void Archive_NoRemainingAmount_ArchivesSuccessfully()
        {
            // Arrange
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                 DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
                );
            var remainingAmount = new Money(0, new Currency("USD"));
            // Act
            goal.Archive(remainingAmount);
            // Assert
            Assert.True(goal.IsArchived);
        }
        [Fact]
        public void Archive_AlreadyArchivedGoal_ThrowsValidationException()
        {
            // Arrange
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                 DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
                );
            var remainingAmount = new Money(0, new Currency("USD"));
            goal.Archive(remainingAmount);
            // Act & Assert
            Assert.Throws<ValidationException>(() => goal.Archive(remainingAmount));
        }
        [Fact]
        public void Archive_WithRemainingAmount_ThrowsValidationException()
        {
            // Arrange
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6))
            );

            var remainingAmount = new Money(500, new Currency("USD"));

            // Act & Assert
            Assert.Throws<ValidationException>(
                () => goal.Archive(remainingAmount));
        }
    }
}
