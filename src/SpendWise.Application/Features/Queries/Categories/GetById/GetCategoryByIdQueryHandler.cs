using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Queries.Categories.GetById;

public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, ICurrentUser currentUser)
    {
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category =
            await _categoryRepository.FindByIdForUserReadOnlyAsync(request.CategoryId, _currentUser.UserId,
                cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found.");

        return new CategoryDto(
            category.Id,
            category.CategoryName,
            category.Description,
            category.CategoryType.ToString(),
            category.IsDefault,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }
}