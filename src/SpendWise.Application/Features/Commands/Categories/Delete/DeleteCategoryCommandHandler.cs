using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Commands.Categories.Delete;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IApplicationDbContext dbContext,
        ICurrentUser currentUser)
    {
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdForUserAsync(
            request.CategoryId,
            _currentUser.UserId,
            cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found.");

        if (category.IsDefault)
            throw new ConflictException("Default categories cannot be deleted.");

        var hasReferences = await _categoryRepository.HasReferencesAsync(category.Id, cancellationToken);

        if (hasReferences)
            throw new ConflictException("Category cannot be deleted because it has financial records.");

        _categoryRepository.Remove(category);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}