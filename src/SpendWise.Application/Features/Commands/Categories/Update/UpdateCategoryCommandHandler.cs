using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Commands.Categories.Update;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext, ICurrentUser currentUser)
    {
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var category = await _categoryRepository.FindByIdForUserAsync(request.CategoryId, userId, cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found.");

        var categoryName = request.CategoryName.Trim();

        var exists = await _categoryRepository.ExistsWithNameAsync(
            userId,
            category.CategoryType,
            categoryName,
            category.Id,
            cancellationToken);

        if (exists)
            throw new ConflictException("A category with the same name and type already exists.");

        category.Rename(categoryName);
        category.UpdateDescription(request.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}