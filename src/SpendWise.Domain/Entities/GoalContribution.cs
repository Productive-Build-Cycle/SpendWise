using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;


namespace SpendWise.Domain.Entities
{
    public class GoalContribution:BaseEntity
    {
       
        public Guid FinancialGoalId { get;private set; }
        public FinancialGoal FinancialGoal { get; private set; } = null!;
        public decimal ContributionAmount { get; private set; }
        public DateOnly ContributionDate { get; private set; }
        public Guid AccountId { get; private set; }
        public Account Account { get; private set; } = null!;
        public Guid? TransactionId { get; private set; }
        public Transactions? Transaction { get; private set; }

        private readonly List<GoalContributionRelease> _releases = new();

        public IReadOnlyCollection<GoalContributionRelease> Releases => _releases.AsReadOnly();


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
                throw new ValidationException("Contribution amount must be greater than zero.");
            }
            if (contributionDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ValidationException("Contribution date cannot be in the future.");
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
        public void AddRelease(GoalContributionRelease release)
        {
            ArgumentNullException.ThrowIfNull(release);
            if(release.GoalContributionId != Id)
            {
                throw new ValidationException(
                    "The release does not belong to this contribution.");
            }
            if (_releases.Any(r => r.Id == release.Id))
            {
                throw new ValidationException(
                    "This release has already been added to the contribution.");
            }
      
            if (release.ReleaseAmount <= 0)
            {
                throw new ValidationException(
                    "The release amount must be greater than zero.");
            }
            decimal remainingAmount=ContributionAmount - _releases.Sum(r=>r.ReleaseAmount);
            if (release.ReleaseAmount > remainingAmount)
            {
                throw new ValidationException(
                    "The release amount exceeds the remaining contribution amount.");
            }
            if (release.ReleaseDate < ContributionDate)
            {
                throw new ValidationException(
                    "The release date cannot be earlier than the contribution date.");
            }
            if(release.ReleaseDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ValidationException(
                    "The release date cannot be in the future.");
            }
            _releases.Add(release);
        }
    }
}
