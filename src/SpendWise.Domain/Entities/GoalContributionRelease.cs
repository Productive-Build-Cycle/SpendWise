using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;


namespace SpendWise.Domain.Entities
{
    public class GoalContributionRelease:BaseEntity
    {
        public Guid GoalContributionId { get;private set; }
        public GoalContribution GoalContribution { get; private set; } = null!;
        public Money ReleaseAmount { get; private set; }
        public DateOnly ReleaseDate { get; private set; }
       
     
        public Guid? TransactionId { get; private set; }
        public Transactions? Transaction { get; private set; } 
        private GoalContributionRelease()
        {
            ReleaseAmount = null!;
        }
        public GoalContributionRelease(Guid goalContributionId,
                                       Money releaseAmount,
                                       DateOnly releaseDate,
                                       Guid? transactionId = null)
        {
            if (transactionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Transaction ID cannot be empty.", nameof(transactionId));
            }
            if (goalContributionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Goal Contribution ID cannot be empty.", nameof(goalContributionId));
            }
            ArgumentNullException.ThrowIfNull(releaseAmount, nameof(releaseAmount));
            if(releaseAmount.Currency.Code != "USD")
            {
                throw new ValidationException(
                    "Release amount must be in USD.");
            };
            if (releaseAmount.Amount <= 0)
            {
                throw new ValidationException(
                    "Release amount must be greater than zero.");
            }
            if (releaseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ValidationException(
                   "Release date cannot be in the future.");
            }
            GoalContributionId = goalContributionId;
            ReleaseAmount = releaseAmount;
            ReleaseDate = releaseDate;
            TransactionId = transactionId;
        }
    }
}
