using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Domain.Tests;

public class CategoryTests
{
    [Theory]
    [InlineData(CategoryType.Income)]
    [InlineData(CategoryType.Expense)]
    public void Constructor_ValidCategory_CreatesSuccessfully(CategoryType categoryType)
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var category = new Category(userId, "Food", categoryType, "Food expenses");

        // Assert
        Assert.Equal(userId, category.UserId);
        Assert.Equal("Food", category.CategoryName);
        Assert.Equal(categoryType, category.CategoryType);
        Assert.Equal("Food expenses", category.Description);
        Assert.False(category.IsDefault);
    }

    [Fact]
    public void Constructor_EmptyUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Category(Guid.Empty, "Food", CategoryType.Expense));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_EmptyCategoryName_ThrowsDomainValidationException(string categoryName)
    {
        Assert.Throws<DomainValidationException>(() =>
            new Category(Guid.NewGuid(), categoryName, CategoryType.Expense));
    }

    [Fact]
    public void Constructor_NullCategoryName_ThrowsDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() => new Category(Guid.NewGuid(), null!, CategoryType.Expense));
    }

    [Fact]
    public void Constructor_CategoryNameExceedsMaxLength_ThrowsDomainValidationException()
    {
        var categoryName = new string('A', 101);

        Assert.Throws<DomainValidationException>(() =>
            new Category(Guid.NewGuid(), categoryName, CategoryType.Expense));
    }

    [Fact]
    public void Constructor_DescriptionExceedsMaxLength_ThrowsDomainValidationException()
    {
        var description = new string('A', 501);

        Assert.Throws<DomainValidationException>(() =>
            new Category(Guid.NewGuid(), "Food", CategoryType.Expense, description));
    }

    [Fact]
    public void Constructor_InvalidCategoryType_ThrowsDomainValidationException()
    {
        var invalidCategoryType = (CategoryType)99;

        Assert.Throws<DomainValidationException>(() => new Category(Guid.NewGuid(), "Food", invalidCategoryType));
    }

    [Fact]
    public void Constructor_ValidCategory_TrimsNameAndDescription()
    {
        // Act
        var category = new Category(Guid.NewGuid(), "  Food  ", CategoryType.Expense, "  Food and groceries  ");

        // Assert
        Assert.Equal("Food", category.CategoryName);
        Assert.Equal("Food and groceries", category.Description);
    }

    [Fact]
    public void Constructor_EmptyDescription_SetsDescriptionToNull()
    {
        var category = new Category(Guid.NewGuid(), "Food", CategoryType.Expense, "   ");

        Assert.Null(category.Description);
    }

    [Fact]
    public void Constructor_DefaultCategory_SetsIsDefaultToFalse()
    {
        var category = new Category(Guid.NewGuid(), "Food", CategoryType.Expense);

        Assert.False(category.IsDefault);
    }

    [Fact]
    public void Constructor_SystemCategory_SetsIsDefaultToTrue()
    {
        var category = new Category(Guid.NewGuid(), "Food", CategoryType.Expense, isDefault: true);

        Assert.True(category.IsDefault);
    }

    [Fact]
    public void Rename_ValidName_UpdatesCategoryName()
    {
        var category = CreateCategory();

        category.Rename("  Transportation  ");

        Assert.Equal("Transportation", category.CategoryName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Rename_EmptyName_ThrowsDomainValidationException(
        string categoryName)
    {
        var category = CreateCategory();

        Assert.Throws<DomainValidationException>(() => category.Rename(categoryName));
    }

    [Fact]
    public void Rename_NameExceedsMaxLength_ThrowsDomainValidationException()
    {
        var category = CreateCategory();

        Assert.Throws<DomainValidationException>(() => category.Rename(new string('A', 101)));
    }

    [Fact]
    public void UpdateDescription_ValidDescription_UpdatesSuccessfully()
    {
        var category = CreateCategory();

        category.UpdateDescription("  Updated description  ");

        Assert.Equal("Updated description", category.Description);
    }

    [Fact]
    public void UpdateDescription_NullDescription_SetsDescriptionToNull()
    {
        var category = CreateCategory();

        category.UpdateDescription(null);

        Assert.Null(category.Description);
    }

    [Fact]
    public void UpdateDescription_WhitespaceDescription_SetsDescriptionToNull()
    {
        var category = CreateCategory();

        category.UpdateDescription("   ");

        Assert.Null(category.Description);
    }

    [Fact]
    public void UpdateDescription_DescriptionExceedsMaxLength_ThrowsDomainValidationException()
    {
        var category = CreateCategory();

        Assert.Throws<DomainValidationException>(() => category.UpdateDescription(new string('A', 501)));
    }

    private static Category CreateCategory()
    {
        return new Category(Guid.NewGuid(), "Food", CategoryType.Expense, "Food expenses");
    }
}