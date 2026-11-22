using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Commands.GoalContributionRelease.Create;
using SpendWise.Api.Contracts.GoalContributionRelease;
using SpendWise.Application.Features.Queries.GoalContributionRelease.GetById;

namespace SpendWise.Api.Controllers
{
    [Authorize]
    [Route("api/goal-contribution-release")]
    [ApiController]
    public class GoalContributionReleaseController : ControllerBase
    {
        private readonly ISender _sender;
        public GoalContributionReleaseController(
          ISender sender
            )
        {
            _sender = sender;
        }
        [HttpPost]
        public async Task<ActionResult<GoalContributionReleaseDto>> Create(
            [FromBody] CreateGoalContributionReleaseRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new CreateGoalContributionReleaseCommand(
                    request.GoalContributionId,
                    request.ReleaseAmount,
                    request.ReleaseDate),
                cancellationToken
                    );
            return Ok(result);
        }
        [HttpGet("{releaseId:guid}")]
        public async Task<ActionResult<GoalContributionReleaseDto>> GetById(
            Guid releaseId,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new GetGoalContributionReleaseByIdQuery(releaseId),
                cancellationToken);

            return Ok(result);

        }
    }
}
