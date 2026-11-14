using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Exceptions;

namespace SpendWise.Application.Features.Commands.Categories.Create;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var typeName = Enum.GetNames<CategoryType>()
            .FirstOrDefault(name =>
                string.Equals(name, request.CategoryType.Trim(), StringComparison.OrdinalIgnoreCase));

        if (typeName is null)
            throw new DomainValidationException("Invalid category type.");

        var categoryType = Enum.Parse<CategoryType>(typeName);

        var userId = _currentUser.UserId;
        var categoryName = request.CategoryName.Trim();

        var exists = await _categoryRepository.ExistsWithNameAsync(
            userId,
            categoryType,
            categoryName,
            cancellationToken: cancellationToken);

        if (exists)
            throw new ConflictException("A category with the same name and type already exists.");

        var category = new Category(
            userId,
            categoryName,
            categoryType,
            request.Description);

        _categoryRepository.Add(category);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}