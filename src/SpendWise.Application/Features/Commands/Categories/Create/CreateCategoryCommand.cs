using MediatR;

namespace SpendWise.Application.Features.Commands.Categories.Create;

public sealed record CreateCategoryCommand(string CategoryName, string CategoryType, string? Description)
    : IRequest<Guid>;