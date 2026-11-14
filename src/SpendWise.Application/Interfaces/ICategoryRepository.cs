using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Category?> FindByIdForUserAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default);

    Task<Category?> FindByIdForUserReadOnlyAsync(Guid categoryId, Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsWithNameAsync(Guid userId, CategoryType categoryType, string categoryName,
        Guid? excludedCategoryId = null, CancellationToken cancellationToken = default);

    Task<bool> HasReferencesAsync(Guid categoryId, CancellationToken cancellationToken = default);

    void Add(Category category);

    void Remove(Category category);
}