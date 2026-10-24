using SpendWise.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Domain.Entities
{
    public class Category: BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string CategoryName { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;
        private Category() { }
        public Category(Guid userId,
                        string categoryName,
                        string? description)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
            }
            if (description != null && description.Length > 500)
            {
                throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            UserId = userId;
            CategoryName = categoryName.Trim();
            Description = string.IsNullOrWhiteSpace(description)
                          ? null
                          :description.Trim();
        }   

    }
}
