using SpendWise.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense
{
    public sealed class IncomeVsExpenseResult
    {
        public Money TotalIncome { get; init; } = null!;
        public Money TotalExpense { get; init; } = null!;
        public decimal NewBalance =>
            TotalIncome.Amount - TotalExpense.Amount;//we do not want to prevent negative amount
    }
}
