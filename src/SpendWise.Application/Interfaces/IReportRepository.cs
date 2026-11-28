using SpendWise.Application.Features.Queries.Reports.GetCategoryBreakDown;
using SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense;
using SpendWise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<IncomeVsExpenseResult> GetIncomeVsExpenseAsync(
            Guid userId,
            DateTime fromeUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default
            );
        Task<IReadOnlyList<CategoryBreakDownResult>> GetCategoryBreakdownAsync(
       Guid userId,
       DateTime fromUtc,
       DateTime toUtc,
       Guid? categoryId,
       TransactionTypes? transactionType,
       CancellationToken cancellationToken = default
       );
    }

}
