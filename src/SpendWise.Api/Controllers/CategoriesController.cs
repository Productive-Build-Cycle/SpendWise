using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Api.Contracts.Categories;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Commands.Categories.Create;
using SpendWise.Application.Features.Commands.Categories.Update;
using SpendWise.Application.Features.Commands.Categories.Delete;
using SpendWise.Application.Features.Queries.Categories.Get;
using SpendWise.Application.Features.Queries.Categories.GetById;

namespace SpendWise.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoriesQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCategoryByIdQuery(categoryId), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var categoryId = await _sender.Send(
            new CreateCategoryCommand(
                request.CategoryName,
                request.CategoryType,
                request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { categoryId }, new { id = categoryId });
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> Update(Guid categoryId, [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCategoryCommand(categoryId, request.CategoryName, request.Description),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Delete(Guid categoryId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCategoryCommand(categoryId), cancellationToken);

        return NoContent();
    }
}