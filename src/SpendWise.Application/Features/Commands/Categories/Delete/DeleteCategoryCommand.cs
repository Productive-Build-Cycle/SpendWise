using MediatR;

namespace SpendWise.Application.Features.Commands.Categories.Delete;

public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Unit>;