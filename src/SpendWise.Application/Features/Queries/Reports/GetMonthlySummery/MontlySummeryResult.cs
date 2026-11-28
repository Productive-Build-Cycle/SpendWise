using SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense;


namespace SpendWise.Application.Features.Queries.Reports.GetMonthlySummery
{
    public sealed record MontlySummeryResult
    (
         int month,
        IncomeVsExpenseResult result

   );
}
