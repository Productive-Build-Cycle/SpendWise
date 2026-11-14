using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Entities
{
    public class Budget : BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        public Money Amount { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }

        private Budget()
        {
            Amount = null!;
        }

        public Budget(Guid userId, Guid categoryId, Money amount, DateOnly startDate, DateOnly endDate)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));

            ArgumentNullException.ThrowIfNull(amount, nameof(amount));
            if (amount.Amount <= 0)
                throw new DomainValidationException("Amount must be a positive value.");

            if (startDate >= endDate)
                throw new DomainValidationException("Start date must be before end date.");

            UserId = userId;
            CategoryId = categoryId;
            Amount = amount;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}