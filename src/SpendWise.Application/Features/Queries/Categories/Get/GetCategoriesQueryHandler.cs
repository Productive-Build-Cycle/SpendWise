using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.Categories.Get;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, ICurrentUser currentUser)
    {
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.ListByUserIdAsync(_currentUser.UserId, cancellationToken);

        return categories.Select(category => new CategoryDto(
            category.Id,
            category.CategoryName,
            category.Description,
            category.CategoryType.ToString(),
            category.IsDefault,
            category.CreatedAtUtc,
            category.UpdatedAtUtc
        )).ToList();
    }
}