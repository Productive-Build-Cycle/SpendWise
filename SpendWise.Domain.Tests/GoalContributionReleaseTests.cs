using SpendWise.Domain.ValueObjects;
using SpendWise.Domain.Entities;

namespace SpendWise.Domain.Tests
{
    public class GoalContributionReleaseTests
    {
        [Fact]
        public void Constructor_ValidRelease_CreateReleaseSuccessfully()
        {// Arrange
            var goalContributionId = Guid.NewGuid();
            var releaseAmount = new Money(100,new Currency("USD"));
            var releaseDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var transactionId = Guid.NewGuid();
            // Act
            var release = new GoalContributionRelease(goalContributionId,
                                                      releaseAmount,
                                                      releaseDate,
                                                      transactionId);
            // Assert
            Assert.Equal(goalContributionId, release.GoalContributionId);
            Assert.Equal(releaseAmount, release.ReleaseAmount);
            Assert.Equal(releaseDate, release.ReleaseDate);
            Assert.Equal(transactionId, release.TransactionId);
        }
    }
}
