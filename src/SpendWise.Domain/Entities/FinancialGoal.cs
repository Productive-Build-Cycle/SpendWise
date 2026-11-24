using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Entities
{
    public class FinancialGoal : BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string GoalName { get; private set; } = string.Empty;
        public Money TargetAmount { get; private set; }
        public DateOnly TargetDate { get; private set; }
        private readonly List<GoalContribution> _contributions = new();
        public IReadOnlyCollection<GoalContribution> Contributions => _contributions.AsReadOnly();

        public Money RemainingAmount => new Money(
            TargetAmount.Amount - _contributions.Sum(
                c => c.RemainingAmount.Amount), TargetAmount.Currency);
        public Money TotalSaved => new Money(
            _contributions.Sum(c => c.RemainingAmount.Amount), TargetAmount.Currency);

        public bool IsArchived { get; private set; } = false;

        private FinancialGoal()
        {
            TargetAmount = null!;
        }
        public FinancialGoal(Guid userId,
                             string goalName,
                             Money targetAmount,
                             DateOnly targetDate,
                             DateOnly currentDate
                            )
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            goalName = ValidateGoalName(goalName);

            ValidateTargetAmount(targetAmount);

            ValidateTargetDate(targetDate, currentDate);

            UserId = userId;
            GoalName = goalName;
            TargetAmount = targetAmount;
            TargetDate = targetDate;

        }
        //...............................................................
        public void Update(string goalName,
            Money targetAmount,
            DateOnly targetDate,
            DateOnly currentDate)
        {
            if (IsArchived)
            {
                throw new DomainValidationException("Cannot update an archived financial goal !");
            }
            goalName = ValidateGoalName(goalName);

            ValidateTargetAmount(targetAmount);

            ValidateTargetDate(targetDate, currentDate);

            GoalName = goalName;
            TargetAmount = targetAmount;
            TargetDate = targetDate;

        }
        public void AddContribution(GoalContribution contribution)
        {
            if (IsArchived)
            {
                throw new DomainValidationException(
                    "Cannot add a contribution to an archived goal.");
            }

            ArgumentNullException.ThrowIfNull(contribution);

            if (contribution.FinancialGoalId != Id)
            {
                throw new DomainValidationException(
                    "This contribution belongs to another financial goal.");
            }

            if (_contributions.Any(c => c.Id == contribution.Id))
            {
                throw new DomainValidationException(
                    "This contribution already exists in this financial goal.");
            }
            if (contribution.ContributionAmount.Currency.Code != TargetAmount.Currency.Code)
            {
                throw new DomainValidationException(
                    "Contribution currency must match the financial goal currency");
            }
            if (contribution.ContributionAmount.Amount > RemainingAmount.Amount)
            {
                throw new DomainValidationException(
                    "Contribution amount exceeds the remaining goal target amount ");
            }

            _contributions.Add(contribution);
        }
        public void Archive()
        {
            if (IsArchived)
            {
                throw new DomainValidationException("Financial goal is already archived.");
            }

            if (TotalSaved.Amount > 0)
            {
                throw new DomainValidationException(
                    "Can not delete a financial goal with remaining contribution money");
            }

            IsArchived = true;
        }
        private static string ValidateGoalName(string goalName)
        {
            if (string.IsNullOrWhiteSpace(goalName))
            {
                throw new ArgumentException("" +
                    "Goal name can not be null or empty!", nameof(goalName));
            }
            goalName = goalName.Trim();
            if (goalName.Length > 100)
            {
                throw new DomainValidationException("Goal name can not exceed 100 characters .");
            }
            return goalName;
        }
        private static void ValidateTargetAmount(Money targetAmount)
        {
            ArgumentNullException.ThrowIfNull(targetAmount);
            if (targetAmount.Amount <= 0)
            {
                throw new DomainValidationException("Target amount must be greater than zero.");
            }
            if (targetAmount.Currency.Code != "USD")
            {
                throw new DomainValidationException(
                    "Financial goal currency in this version is only USD .");
            }
        }

        private static void ValidateTargetDate(DateOnly targetDate,
            DateOnly currentDate)
        {
            if (targetDate <= currentDate)
            {
                throw new DomainValidationException("Target date must be in the future.");
            }
        }
    }
}
