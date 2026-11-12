using SpendWise.Domain.Common;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Domain.Entities
{
    public class Category: BaseEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public string CategoryName { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        private Category() { }
        public Category(Guid userId,
                        string categoryName,
                        string? description)
        {
            if(userId==Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
            }
            categoryName = categoryName.Trim();
            if (categoryName.Length > 100)
            {
                throw new DomainValidationException("Category name cannot exceed 100 characters.");
            }
            if (description != null && description.Length > 500)
            {
                throw new DomainValidationException("Description cannot exceed 500 characters.");
            }
           
            UserId = userId;
            CategoryName = categoryName;
            Description = string.IsNullOrWhiteSpace(description)
                          ? null
                          :description.Trim();
        }   

    }
}
