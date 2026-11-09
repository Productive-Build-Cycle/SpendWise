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
            var contribution = new GoalContribution(
                financialGoalId: Guid.NewGuid(),
                contributionAmount: new Money(1000, new Currency("USD")),
                contributionDate: DateOnly.FromDateTime(DateTime.UtcNow),
                accountId: Guid.NewGuid()
            );
            var firstRelease = new GoalContributionRelease(
                 goalContributionId: contribution.Id,
                 releaseAmount: new Money(700, new Currency("USD")),
                 releaseDate: DateOnly.FromDateTime(DateTime.UtcNow)
             );
            contribution.AddRelease(firstRelease);
            var secoundRelease = new GoalContributionRelease(
                goalContributionId: contribution.Id,
                releaseAmount: new Money(400, new Currency("USD")),
                releaseDate: DateOnly.FromDateTime(DateTime.UtcNow)
            );
            // Act & Assert
            Assert.Throws<ValidationException>(() =>
            {
                contribution.AddRelease(secoundRelease);
            });
        }
        [Fact]
        public void AddRelease_ValidRelease_AddsReleaseSuccessfully()
        {
            // Arrange
            var contribution = new GoalContribution(
                financialGoalId: Guid.NewGuid(),
                contributionAmount: new Money(1000, new Currency("USD")),
                contributionDate: DateOnly.FromDateTime(DateTime.UtcNow),
                accountId: Guid.NewGuid()
            );
            var release = new GoalContributionRelease(
                goalContributionId: contribution.Id,
                releaseAmount: new Money(500, new Currency("USD")),
                releaseDate: DateOnly.FromDateTime(DateTime.UtcNow)
            );
            // Act
            contribution.AddRelease(release);
            // Assert
            Assert.Contains(release, contribution.Releases);
        }

        [Fact]
        public void RemainingAmount_AfterAddingRelease_ReturnsCorrectRemainingAmount()
        {
            // Arrange
            var contribution = new GoalContribution(
                financialGoalId: Guid.NewGuid(),
                contributionAmount: new Money(1000, new Currency("USD")),
                contributionDate: DateOnly.FromDateTime(DateTime.UtcNow),
                accountId: Guid.NewGuid()
            );
            var release = new GoalContributionRelease(
                goalContributionId: contribution.Id,
                releaseAmount: new Money(300, new Currency("USD")),
                releaseDate: DateOnly.FromDateTime(DateTime.UtcNow)
            );
            // Act
            contribution.AddRelease(release);
            // Assert
            Assert.Equal(700, contribution.RemainingAmount.Amount);
        }


    }
}
