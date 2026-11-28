using MediatR;
using SpendWise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Features.Queries.Reports.GetCategoryBreakDown
{
    public sealed record GetCategoryBreakDownQuesry(
        DateOnly fromDate,
        DateOnly toDate,
        Guid? categoryId,
        TransactionTypes? transactionType
        ) : IRequest<IReadOnlyList<CategoryBreakDownResult>>;
}
