using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Api.Contracts.FinancialGoals;
using SpendWise.Application.Features.Commands.FinancialGoals.Create;
using SpendWise.Application.Features.Commands.FinancialGoals.Update;

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
        public async Task<IActionResult> CreateFinancialGoal(
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
    }
}
