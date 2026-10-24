using SpendWise.Domain.Common;


namespace SpendWise.Domain.Entities
{
   public class FinancialGoal:BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string GoalName { get; private set; } = string.Empty;
        public decimal TargetAmount { get; private set; }
        public DateOnly TargetDate { get; private set; }
        public ICollection<GoalContribution> Contributions { get; private set; } = new List<GoalContribution>();

        public bool IsArchived { get; private set; } = false;

        private FinancialGoal()
        {

        }
        public FinancialGoal(Guid userId, 
                             string goalName, 
                             decimal targetAmount, 
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
            if (targetAmount <= 0)
            {
                throw new ArgumentException("Target amount must be greater than zero.", nameof(targetAmount));
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
        public void Archive()
        {
            IsArchived = true;
        }
    }
}
