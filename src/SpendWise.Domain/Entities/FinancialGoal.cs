using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Entities
{
   public class FinancialGoal:BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string GoalName { get; private set; } = string.Empty;
        public Money TargetAmount { get; private set; } 
        public DateOnly TargetDate { get; private set; }
        private readonly List<GoalContribution> _contributions = new();
        public IReadOnlyCollection<GoalContribution> Contributions => _contributions.AsReadOnly();

        public bool IsArchived { get; private set; } = false;

        private FinancialGoal()
        {
            TargetAmount = null!;
        }
        public FinancialGoal(Guid userId, 
                             string goalName, 
                             Money targetAmount, 
                             DateOnly targetDate
                            )
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(goalName))
            {
                throw new ArgumentException("Goal name cannot be null or empty.", nameof(goalName));
            }
            if(goalName.Length > 100)
            {
                throw new ValidationException("Goal name cannot exceed 100 characters.");
            }

           ArgumentNullException.ThrowIfNull(targetAmount, nameof(targetAmount));
            if (targetAmount.Amount <= 0)
            {
                throw new ValidationException(
                                "Target amount must be greater than zero." );
            }
            if (targetAmount.Currency.Code != "USD")
            {
                throw new ValidationException(
                    $"Financial goal currency in this version is only USD .");
            }
            if (targetDate <=DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException("Target date must be in the future.", nameof(targetDate));
            }
            UserId = userId;
            GoalName = goalName.Trim();
            TargetAmount = targetAmount;
            TargetDate = targetDate;
        }
        //...............................................................
        public void AddContribution(GoalContribution contribution)
        {
            if (IsArchived)
            {
                throw new ValidationException(
                    "Cannot add a contribution to an archived goal.");
            }

            ArgumentNullException.ThrowIfNull(contribution);

            if (contribution.FinancialGoalId != Id)
            {
                throw new ValidationException(
                    "This contribution belongs to another financial goal.");
            }

            if (_contributions.Any(c => c.Id == contribution.Id))
            {
                throw new ValidationException(
                    "This contribution already exists in this financial goal.");
            }

            _contributions.Add(contribution);
        }
        public void Archive(Money remainingAllocateAmount)
        {
            if (IsArchived)
            {
                throw new ValidationException("Financial goal is already archived.");
            }
            if (remainingAllocateAmount.Amount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(remainingAllocateAmount),
                    "Remaining allocated amount cannot be negative.");
            }
            if (remainingAllocateAmount.Amount > 0)
            {
                throw new ValidationException(
                    "Cannot archive the financial goal with remaining allocated amount.");
            }
            IsArchived = true;
        }
    }
}
