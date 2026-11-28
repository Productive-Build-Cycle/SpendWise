using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Features.Queries.Reports.GetCategoryBreakDown;
using SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense;
using SpendWise.Application.Features.Queries.Reports.GetMonthlySummery;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Enums;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Infrastructure.Persistence.Repositories
{
    public class ReportRepository
        : IReportRepository,
        IReportMontlySummeryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ReportRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IncomeVsExpenseResult> GetIncomeVsExpenseAsync(
            Guid userId,
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default)
        {
            var total = await _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId &&
                t.TransactionDate >= fromUtc && t.TransactionDate < toUtc)
                .GroupBy(t => 1)
                .Select(g => new
                {
                    Income = g
                    .Where(t => t.TransactionType == TransactionTypes.Income)
                    .Sum(t => t.Amount.Amount),
                    Expense = g
                    .Where(t => t.TransactionType == TransactionTypes.Expense)
                    .Sum(t => t.Amount.Amount)
                })
                .SingleOrDefaultAsync(cancellationToken);
            return new IncomeVsExpenseResult
            {
                TotalIncome = new Money(total?.Income ?? 0m, new Currency("USD")),
                TotalExpense = new Money(total?.Expense ?? 0m, new Currency("USD"))
            };
        }
        public async Task<IReadOnlyList<MontlySummeryResult>> GetMontlySummeryAsync(
            Guid userId,
            string timeZoneID,
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken)
        {
            var transactions = _dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId &&
                t.TransactionDate >= fromUtc &&
                t.TransactionDate < toUtc);
            var monthlyTotal = await transactions
                .GroupBy(t => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                    t.TransactionDate, timeZoneID).Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Income = g
                    .Where(t => t.TransactionType == TransactionTypes.Income)
                    .Sum(t => t.Amount.Amount),
                    Expense = g
                    .Where(t => t.TransactionType == TransactionTypes.Expense)
                    .Sum(t => t.Amount.Amount)
                }).ToListAsync(cancellationToken);
            var result = new List<MontlySummeryResult>();
            foreach (var item in monthlyTotal)
            {
                result.Add(new MontlySummeryResult(
                    item.Month,
                    new IncomeVsExpenseResult
                    {
                        TotalIncome = new Money(item.Income, new Currency("USD")),
                        TotalExpense = new Money(item.Expense, new Currency("USD"))
                    })
                        );
            }
            return result;
        }
        public async Task<IReadOnlyList<CategoryBreakDownResult>>
            GetCategoryBreakdownAsync(
            Guid userId,
            DateTime fromUtc,
            DateTime toUtc,
            Guid? CategoryId,
            TransactionTypes? type,
            CancellationToken cancellationToken = default
            )
        {
            var query = _dbContext.Transactions
                .AsNoTracking()
                .Where(t =>
                t.UserId == userId &&
                t.TransactionDate >= fromUtc &&
                t.TransactionDate < toUtc &&
                t.TransactionType != TransactionTypes.Transfer);
            if (CategoryId.HasValue)
            {
                query = query.Where(q => q.CategoryId == CategoryId);
            }
            if (type.HasValue)
            {
                query = query.Where(q => q.TransactionType == type.Value);
            }
            var result = await query
                .GroupBy(t => new
                {
                    t.CategoryId,
                    t.Category!.CategoryName,
                    t.TransactionType
                })
                .Select(g => new CategoryBreakDownResult
                {
                    CategoryId = g.Key.CategoryId.Value!,
                    CategoryName = g.Key.CategoryName,
                    Type = g.Key.TransactionType,
                    TotalAmount = g.Sum(t => t.Amount.Amount)
                })
                .ToListAsync(cancellationToken);
            return result;
        }
    }
}
