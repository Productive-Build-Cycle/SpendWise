using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;


namespace SpendWise.Domain.Entities
{
    public class GoalContributionRelease:BaseEntity
    {
        public Guid GoalContributionId { get;private set; }
        public GoalContribution GoalContribution { get; private set; } = null!;
        public decimal ReleaseAmount { get; private set; }
        public DateOnly ReleaseDate { get; private set; }
       
     
        public Guid? TransactionId { get; private set; }
        public Transactions? Transaction { get; private set; } 
        private GoalContributionRelease()
        {
        }
        public GoalContributionRelease(Guid goalContributionId,
                                       decimal releaseAmount,
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
            if (releaseAmount <= 0)
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
