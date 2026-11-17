using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Api.Contracts.Transactions;
using SpendWise.Application.Common.Models;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Commands.Transactions.Create;
using SpendWise.Application.Features.Queries.Transactions.Get;
using SpendWise.Application.Features.Queries.Transactions.GetById;
using SpendWise.Domain.Enums;

namespace SpendWise.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly ISender _sender;

    public TransactionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? type = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetTransactionsQuery(pageNumber, pageSize, fromDate, toDate, categoryId, type),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{transactionId:guid}")]
    public async Task<ActionResult<TransactionDto>> GetById(Guid transactionId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTransactionByIdQuery(transactionId), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var transactionId = await _sender.Send(
            new CreateTransactionCommand(
                request.TransactionType,
                request.SourceAccountId,
                request.DestinationAccountId,
                request.CategoryId,
                request.Amount,
                request.Description,
                request.TransactionDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { transactionId }, new { id = transactionId });
    }

    [HttpPost("income")]
    public async Task<IActionResult> CreateIncome([FromBody] CreateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var transactionId = await _sender.Send(
            new CreateTransactionCommand(
                nameof(TransactionTypes.Income),
                null,
                request.DestinationAccountId,
                request.CategoryId,
                request.Amount,
                request.Description,
                request.TransactionDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { transactionId }, new { id = transactionId });
    }

    [HttpPost("expense")]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var transactionId = await _sender.Send(
            new CreateTransactionCommand(
                nameof(TransactionTypes.Expense),
                request.SourceAccountId,
                null,
                request.CategoryId,
                request.Amount,
                request.Description,
                request.TransactionDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { transactionId }, new { id = transactionId });
    }
}
