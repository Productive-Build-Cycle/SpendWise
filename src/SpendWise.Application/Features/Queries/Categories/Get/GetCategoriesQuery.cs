using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Categories.Get;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;