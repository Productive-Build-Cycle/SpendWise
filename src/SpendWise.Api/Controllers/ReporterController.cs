using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense;
using Microsoft.AspNetCore.Authorization;
using SpendWise.Application.Features.Queries.Reports.GetMonthlySummery;
using SpendWise.Application.Features.Queries.Reports.GetCategoryBreakDown;


namespace SpendWise.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reports")]
    public class ReporterController : ControllerBase
    {
        private readonly ISender _sender;
        public ReporterController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet("income-vs-expense")]
        public async Task<ActionResult<IncomeVsExpenseDto>> GetIncomeVsExpense(
            [FromQuery] GetIncomeVsExpenseQuery query,
            CancellationToken cancellationToken = default)
        {

            var result = await _sender.Send(query, cancellationToken);
            var dto = new IncomeVsExpenseDto(
                result.TotalIncome.Amount,
                result.TotalExpense.Amount,
                result.NewBalance,
                result.TotalIncome.Currency.Code
                );
            return Ok(dto);
        }
        [HttpGet("monthly-summery")]
        public async Task<IReadOnlyList<MontlySummeryResult>> GetMontlysummery(
            [FromQuery] int year,
            CancellationToken cancellationToken = default)
        {
            return await _sender.Send(new GetMontlySummeryQuery(year),
                cancellationToken);
        }

        [HttpGet("category-breakdown")]
        public async Task<ActionResult<IReadOnlyList<CategoryBreakdownDto>>>
            GetCategoryBreakdown([FromQuery] GetCategoryBreakDownQuesry query,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            var dto = result.Select(item => new CategoryBreakdownDto(
                item.CategoryId,
                item.CategoryName,
                item.Type,
                item.TotalAmount
                )).ToList();
            return dto;
        }
    }
}
