using SpendWise.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Domain.Entities
{
    public class Budget:BaseEntity
    {
        public Guid UserId { get;private set; }
        public User User { get; private set; } = null!;
        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        public decimal Amount { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        private Budget() { }
        public Budget(Guid userId,
                       Guid categoryId,
                       decimal amount,
                       DateOnly startDate,
                       DateOnly endDate)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be a positive value.", nameof(amount));
            }
            if (startDate >= endDate)
            {
                throw new ArgumentException("Start date must be before end date.", nameof(startDate));
            }
            UserId = userId;
            CategoryId = categoryId;
            Amount = amount;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
