using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Commands.GoalContributions.Create;
using SpendWise.Api.Contracts.GoalContributions;
using SpendWise.Application.Features.Queries.GoalContributions.GetByFinancialGoalId;


namespace SpendWise.Api.Controllers
{
    [Authorize]
    [Route("api/goal-contributions")]
    [ApiController]
    public sealed class GoalContributionController : ControllerBase
    {
        private readonly ISender _sender;

        public GoalContributionController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<ActionResult<GoalContributionDto>> Create(
            [FromBody] CreateGoalContributionRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateGoalContributionCommand(
                    request.FinancialGoalId,
                    request.AccountId,
                    request.SourceAccountId,
                    request.Amount,
                    request.ContributionDate),
                cancellationToken);
            return Ok(result);
        }
        [HttpGet("{goalId:guid}")]
        public async Task<ActionResult<IReadOnlyList<GoalContributionDto>>> GetByFinancialGoalId(Guid goalId,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new GetGoalContributionByGoalIdQuery(goalId), cancellationToken);
            return Ok(result);
        }
    }
}
