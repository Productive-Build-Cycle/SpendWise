using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense
{
    public sealed record GetIncomeVsExpenseQuery
    (
        DateOnly FromDate,
        DateOnly ToDate
        ) : IRequest<IncomeVsExpenseResult>;
}
