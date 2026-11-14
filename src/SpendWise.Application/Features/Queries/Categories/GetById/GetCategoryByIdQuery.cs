using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Categories.GetById;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IRequest<CategoryDto>;