using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Api.Contracts.FinancialGoals;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Commands.FinancialGoals.Archive;
using SpendWise.Application.Features.Commands.FinancialGoals.Create;
using SpendWise.Application.Features.Commands.FinancialGoals.Update;
using SpendWise.Application.Features.Queries.FinancialGoals.GetAll;
using SpendWise.Application.Features.Queries.FinancialGoals.GetById;
using SpendWise.Application.Features.Queries.GoalContributions.GetByFinancialGoalId;

namespace SpendWise.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/financial-goals")]
    public sealed class FinancialGoalsController : ControllerBase
    {
        private readonly ISender _sender;
        public FinancialGoalsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateFinancialGoalRequest request,
            CancellationToken cancellationToken)
        {
            var goalId = await _sender.Send(new CreateFinancialGoalCommand(
                request.GoalName,
                request.TargetAmount,
                request.CurrencyCode,
                request.TargetDate),
                cancellationToken);
            return Ok(goalId);
        }

        [HttpPut]
        [Route("{goalId:guid}")]

        public async Task<IActionResult> UpdateFinancialGoalByIdAsync(
            [FromRoute] Guid goalId,
            [FromBody] UpdateFinancialGoalRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new UpdateFinancialGoalCommand(
               goalId,
                request.GoalName,
                request.TargetAmount,
                request.CurrencyCode,
                request.TargetDate),
                cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<FinancialGoalDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetAllFinancialGoalsQuery(),
                cancellationToken);
            return Ok(result);
        }

        [HttpGet("{goalId:guid}")]
        public async Task<ActionResult<FinancialGoalDto>> GetById(
            Guid goalId, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetFinancialGoalByIdQuery(goalId),
                cancellationToken);
            return Ok(result);
        }

        [HttpGet("{financialGoalId}/Contribution")]
        public async Task<ActionResult<IReadOnlyList<GoalContributionDto>>> GetGoalContributionByFinancialGoalId
            (Guid financialGoalId, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetGoalContributionByGoalIdQuery(financialGoalId),
                cancellationToken);
            return Ok(result);
        }

        [HttpPost("{goalId:guid}/archive")]
        public async Task<IActionResult> Archive(Guid goalId,
            CancellationToken cancellationToken = default)
        {
            await (_sender.Send(new ArchiveFinancialGoalCommand(goalId),
                cancellationToken));

            return NoContent();
        }
    }
}
