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
            var currentDate = new DateOnly(2026, 11, 15);
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                currentDate.AddMonths(6),
                currentDate);

            var contributionDate = new DateOnly(2026, 11, 15);

            var contribution = new GoalContribution(
                goal.Id,
                new Money(1000, new Currency("USD")),
                contributionDate,
                currentDate,
                Guid.NewGuid()
            );
            goal.AddContribution(contribution);
            //Act & Assert
            Assert.Throws<DomainValidationException>(() => goal.AddContribution(contribution));
        }

        [Fact]
        public void AddContribution_IsArchivedGoal_ThrowsValidationException()
        {
            // Arrange
            var currentDate = new DateOnly(2026, 11, 15);
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                currentDate.AddMonths(6),
                currentDate);

            var contributionDate = new DateOnly(2026, 11, 15);

            var contribution = new GoalContribution(
                goal.Id,
                new Money(1000, new Currency("USD")),
                contributionDate,
                currentDate,
                Guid.NewGuid()
            );
            var remainingAmount = new Money(0, new Currency("USD"));
            goal.Archive();
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => goal.AddContribution(contribution));
        }
        [Fact]
        public void Archive_FinancialGoalWithRemainingAmount_ThrowsValidationException()
        {
            // Arrange
            var currentDate = new DateOnly(2026, 11, 15);
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                currentDate.AddMonths(5),
                currentDate);

            goal.Archive();

            var contributionDate = new DateOnly(2023, 11, 15);
            var contribution = new GoalContribution(
                goal.Id,
                new Money(1000, new Currency("USD")),
                contributionDate,
                currentDate,
                Guid.NewGuid()//AccountId
            );
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => goal.AddContribution(contribution));
        }
        [Fact]
        public void Archive_NoRemainingAmount_ArchivesSuccessfully()
        {
            // Arrange
            var currentDate = new DateOnly(2023, 11, 15);
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                 currentDate.AddMonths(5),
                 currentDate);
            var remainingAmount = new Money(0, new Currency("USD"));
            // Act
            goal.Archive();
            // Assert
            Assert.True(goal.IsArchived);
        }
        [Fact]
        public void Archive_AlreadyArchivedGoal_ThrowsValidationException()
        {
            // Arrange
            var currentDate = new DateOnly(2023, 11, 15);
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                 currentDate.AddMonths(5),
                 currentDate);
            var remainingAmount = new Money(0, new Currency("USD"));
            goal.Archive();
            // Act & Assert
            Assert.Throws<DomainValidationException>(() => goal.Archive());
        }
        [Fact]
        public void Archive_WithRemainingAmount_ThrowsValidationException()
        {
            // Arrange
            var currentDate = new DateOnly(2023, 11, 15);
            var goal = new FinancialGoal(
                Guid.NewGuid(),
                "Buy a car",
                new Money(10000, new Currency("USD")),
                currentDate.AddMonths(6),
                currentDate);

            var contribution = new GoalContribution(
                goal.Id,
                 new Money(500, new Currency("USD")),
                 currentDate,
                 currentDate,
                 Guid.NewGuid());

            goal.AddContribution(contribution);

            // Act & Assert
            Assert.Throws<DomainValidationException>(
                () => goal.Archive());
        }
    }
}
