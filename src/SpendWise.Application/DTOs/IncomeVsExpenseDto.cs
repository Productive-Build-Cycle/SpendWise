using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.DTOs
{
    public sealed record IncomeVsExpenseDto
    (
        decimal TotalIncome,
        decimal TotalExpense,
        decimal NewBalance,
        string Currency

   );
}