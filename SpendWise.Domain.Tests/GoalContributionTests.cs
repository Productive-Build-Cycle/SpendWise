using SpendWise.Domain.Entities;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Tests
{
    public class GoalContributionTests
    {
        [Fact]
        public void AddRelease_AmountExceedsContribution_ThrowsValidationException()
        {
            // Arrange
            var currentDate = new DateOnly(2026, 11, 15);
            var contributionDate = new DateOnly(2026, 11, 15);
            var contribution = new GoalContribution(
                financialGoalId: Guid.NewGuid(),
                contributionAmount: new Money(1000, new Currency("USD")),
                contributionDate,
                currentDate,
                accountId: Guid.NewGuid()
            );
            var firstRelease = new GoalContributionRelease(
                 goalContributionId: contribution.Id,
                 releaseAmount: new Money(700, new Currency("USD")),
                 releaseDate: currentDate,
                 currentDate: currentDate

             );
            contribution.AddRelease(firstRelease, currentDate);
            var secoundRelease = new GoalContributionRelease(
                goalContributionId: contribution.Id,
                releaseAmount: new Money(400, new Currency("USD")),
                releaseDate: currentDate,
                currentDate: currentDate
            );
            // Act & Assert
            Assert.Throws<DomainValidationException>(() =>
            {
                contribution.AddRelease(secoundRelease, currentDate);
            });
        }
        [Fact]
        public void AddRelease_ValidRelease_AddsReleaseSuccessfully()
        {
            // Arrange
            var currentDate = new DateOnly(2026, 11, 15);
            var contributionDate = new DateOnly(2026, 11, 15);
            var contribution = new GoalContribution(
                financialGoalId: Guid.NewGuid(),
                contributionAmount: new Money(1000, new Currency("USD")),
                contributionDate,
                currentDate,
                accountId: Guid.NewGuid()
            );
            var release = new GoalContributionRelease(
                goalContributionId: contribution.Id,
                releaseAmount: new Money(500, new Currency("USD")),
                releaseDate: currentDate,
                currentDate: currentDate
            );
            // Act
            contribution.AddRelease(release, currentDate);
            // Assert
            Assert.Contains(release, contribution.Releases);
        }

        [Fact]
        public void RemainingAmount_AfterAddingRelease_ReturnsCorrectRemainingAmount()
        {
            // Arrange
            var currentDate = new DateOnly(2026, 11, 15);
            var contributionDate = new DateOnly(2026, 11, 15);
            var contribution = new GoalContribution(
                financialGoalId: Guid.NewGuid(),
                contributionAmount: new Money(1000, new Currency("USD")),
               contributionDate,
               currentDate,
                accountId: Guid.NewGuid()
            );
            var release = new GoalContributionRelease(
                goalContributionId: contribution.Id,
                releaseAmount: new Money(300, new Currency("USD")),
                releaseDate: currentDate,
                currentDate: currentDate
            );
            // Act
            contribution.AddRelease(release, currentDate);
            // Assert
            Assert.Equal(700, contribution.RemainingAmount.Amount);
        }


    }
}
