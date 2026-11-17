using MediatR;
using SpendWise.Application.Common.Models;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.Transactions.Get;

public sealed record GetTransactionsQuery(
    int PageNumber,
    int PageSize,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? CategoryId,
    string? Type) : IRequest<PagedResult<TransactionDto>>;
