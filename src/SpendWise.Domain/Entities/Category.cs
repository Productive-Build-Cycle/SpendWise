using SpendWise.Domain.Common;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Domain.Entities;

public class Category : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string CategoryName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public CategoryType CategoryType { get; private set; }

    public bool IsDefault { get; private set; }

    private Category()
    {
    }

    public Category(Guid userId, string categoryName, CategoryType categoryType, string? description = null,
        bool isDefault = false)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ValidateCategoryName(categoryName);
        ValidateDescription(description);

        if (!Enum.IsDefined(categoryType))
            throw new DomainValidationException("Invalid category type.");

        UserId = userId;
        CategoryName = categoryName.Trim();
        Description = NormalizeDescription(description);
        CategoryType = categoryType;
        IsDefault = isDefault;
    }

    public void Rename(string categoryName)
    {
        ValidateCategoryName(categoryName);
        CategoryName = categoryName.Trim();
    }

    public void UpdateDescription(string? description)
    {
        ValidateDescription(description);
        Description = NormalizeDescription(description);
    }

    private static void ValidateCategoryName(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new DomainValidationException("Category name cannot be empty.");

        if (categoryName.Trim().Length > 100)
            throw new DomainValidationException("Category name cannot exceed 100 characters.");
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Trim().Length > 500)
            throw new DomainValidationException("Description cannot exceed 500 characters.");
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}