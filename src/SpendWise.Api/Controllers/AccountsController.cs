using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Api.Contracts.Accounts;
using SpendWise.Application.DTOs;
using SpendWise.Application.Features.Commands.Accounts.Create;
using SpendWise.Application.Features.Commands.Accounts.Update;
using SpendWise.Application.Features.Commands.Accounts.Delete;
using SpendWise.Application.Features.Queries.Accounts.Get;
using SpendWise.Application.Features.Queries.Accounts.GetById;
using SpendWise.Application.Features.Queries.Accounts.GetBalance;

namespace SpendWise.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly ISender _sender;

    public AccountsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountListItemDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAccountsQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{accountId:guid}")]
    public async Task<ActionResult<AccountDetailsDto>> GetById(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAccountByIdQuery(accountId), cancellationToken);

        return Ok(result);
    }

    [HttpGet("{accountId:guid}/balance")]
    public async Task<ActionResult<AccountBalanceDto>> GetBalance(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAccountBalanceQuery(accountId), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var accountId = await _sender.Send(
            new CreateAccountCommand(
                request.AccountName,
                request.AccountType,
                request.OpeningBalance,
                request.CreditLimit),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { accountId }, new { id = accountId });
    }

    [HttpPut("{accountId:guid}")]
    public async Task<IActionResult> Update(Guid accountId, [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateAccountCommand(accountId, request.AccountName, request.CreditLimit), cancellationToken);

        return NoContent();
    }

    [HttpDelete("{accountId:guid}")]
    public async Task<IActionResult> Delete(Guid accountId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteAccountCommand(accountId), cancellationToken);

        return NoContent();
    }
}
