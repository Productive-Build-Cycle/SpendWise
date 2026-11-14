using MediatR;

namespace SpendWise.Application.Features.Commands.Categories.Update;

public sealed record UpdateCategoryCommand(Guid CategoryId, string CategoryName, string? Description) : IRequest<Unit>;