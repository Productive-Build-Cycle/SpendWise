using SpendWise.Domain.Common;


namespace SpendWise.Domain.Entities
{
    public class GoalContribution:BaseEntity
    {
       
        public Guid FinancialGoalId { get;private set; }
        public FinancialGoal FinancialGoal { get; private set; } 
        public decimal ContributionAmount { get; private set; }
        public DateOnly ContributionDate { get; private set; }
        public Guid AccountId { get; private set; }
        public Account Account { get; private set; } 
        public Guid? TransactionId { get; private set; }
        public Transactions? Transaction { get; private set; }

        public ICollection<GoalContributionRelease> Releases { get; private set; }
         = new List<GoalContributionRelease>();

        private GoalContribution()
        {
        }
        public GoalContribution(Guid financialGoalId,
                                decimal contributionAmount,
                                DateOnly contributionDate,
                                Guid accountId,
                                Guid? transactionId = null)
        {
            if (transactionId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Transaction ID cannot be empty.", nameof(transactionId));
            }
            if (financialGoalId == Guid.Empty)
            {
                throw new ArgumentException("Financial goal ID cannot be empty.", nameof(financialGoalId));
            }
            if (contributionAmount <= 0)
            {
                throw new ArgumentException("Contribution amount must be greater than zero.", nameof(contributionAmount));
            }
            if (contributionDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Contribution date cannot be in the future.", nameof(contributionDate));
            }
            if (accountId == Guid.Empty)
            {
                throw new ArgumentException("Account ID cannot be empty.", nameof(accountId));
            }
            FinancialGoalId = financialGoalId;
            ContributionAmount = contributionAmount;
            ContributionDate = contributionDate;
            AccountId = accountId;
            TransactionId = transactionId;
        }
    }
}
