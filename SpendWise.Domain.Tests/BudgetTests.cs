using SpendWise.Domain.Entities;
using SpendWise.Domain.Exceptions;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Domain.Tests;

public class BudgetTests
{
    private static Money CreateMoney(decimal amount)
    {
        return new Money(amount, new Currency("USD"));
    }

    private static Budget CreateValidBudget(
        Guid? userId = null,
        Guid? categoryId = null,
        Money? amount = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        var start = startDate ?? new DateOnly(2026, 1, 1);

        return new Budget(
            userId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid(),
            amount ?? CreateMoney(1000),
            start,
            endDate ?? start.AddMonths(1));
    }

    [Fact]
    public void Constructor_WithValidData_ShouldCreateBudget()
    {
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var amount = CreateMoney(1000);
        var startDate = new DateOnly(2026, 1, 1);
        var endDate = new DateOnly(2026, 2, 1);

        var budget = new Budget(
            userId,
            categoryId,
            amount,
            startDate,
            endDate);

        Assert.Equal(userId, budget.UserId);
        Assert.Equal(categoryId, budget.CategoryId);
        Assert.Equal(amount.Amount, budget.Amount.Amount);
        Assert.Equal(startDate, budget.StartDate);
        Assert.Equal(endDate, budget.EndDate);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            CreateValidBudget(userId: Guid.Empty));
    }

    [Fact]
    public void Constructor_WithEmptyCategoryId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            CreateValidBudget(categoryId: Guid.Empty));
    }

    [Fact]
    public void Constructor_WithNullAmount_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Budget(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null!,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 2, 1)));
    }

    [Fact]
    public void Constructor_WithZeroAmount_ShouldThrowDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() =>
            CreateValidBudget(amount: CreateMoney(0)));
    }

    [Fact]
    public void Constructor_WithEndDateBeforeStartDate_ShouldThrowDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() =>
            CreateValidBudget(
                startDate: new DateOnly(2026, 2, 1),
                endDate: new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public void Constructor_WithSameStartAndEndDate_ShouldThrowDomainValidationException()
    {
        var date = new DateOnly(2026, 1, 1);

        Assert.Throws<DomainValidationException>(() =>
            CreateValidBudget(
                startDate: date,
                endDate: date));
    }
}