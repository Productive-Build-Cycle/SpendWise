using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Category>> ListByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.CategoryType)
            .ThenBy(c => c.CategoryName)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> FindByIdForUserAsync(Guid categoryId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.SingleOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId,
            cancellationToken);
    }

    public Task<Category?> FindByIdForUserReadOnlyAsync(Guid categoryId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(
        Guid userId,
        CategoryType categoryType,
        string categoryName,
        Guid? excludedCategoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.CategoryType == categoryType && c.CategoryName == categoryName);

        if (excludedCategoryId.HasValue)
        {
            var excludedId = excludedCategoryId.Value;

            query = query.Where(c => c.Id != excludedId);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasReferencesAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Budgets.AnyAsync(b => b.CategoryId == categoryId, cancellationToken))
            return true;

        if (await _dbContext.Transactions.AnyAsync(t => t.CategoryId == categoryId, cancellationToken))
            return true;

        return await _dbContext.RecurringTransactions.AnyAsync(rt => rt.CategoryId == categoryId, cancellationToken);
    }

    public void Add(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _dbContext.Categories.Add(category);
    }

    public void Remove(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _dbContext.Categories.Remove(category);
    }
}