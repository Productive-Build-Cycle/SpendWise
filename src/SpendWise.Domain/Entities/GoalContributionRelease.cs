using SpendWise.Domain.Common;


namespace SpendWise.Domain.Entities
{
    public class GoalContributionRelease:BaseEntity
    {
        public Guid GoalContributionId { get;private set; }
        public GoalContribution GoalContribution { get; private set; } = null!;
        public decimal ReleaseAmount { get; private set; }
        public DateTime ReleaseDate { get; private set; }
       
     
        public Guid? TransactionId { get; private set; }
        public Transactions? Transaction { get; private set; } 
        private GoalContributionRelease()
        {
        }
        public GoalContributionRelease(Guid goalContributionId,
                                       decimal releaseAmount,
                                       DateTime releaseDate,
                                       Guid? transactionId = null)
        {
            if (transactionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Transaction ID cannot be empty.", nameof(transactionId));
            }
            if (goalContributionId == Guid.Empty)
            {
                throw new ArgumentException("Goal Contribution ID cannot be empty.", nameof(goalContributionId));
            }
            if (releaseAmount <= 0)
            {
                throw new ArgumentException("Release amount must be greater than zero.", nameof(releaseAmount));
            }
            if (releaseDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Release date cannot be in the future.", nameof(releaseDate));
            }
            GoalContributionId = goalContributionId;
            ReleaseAmount = releaseAmount;
            ReleaseDate = releaseDate;
            TransactionId = transactionId;
        }
    }
}
