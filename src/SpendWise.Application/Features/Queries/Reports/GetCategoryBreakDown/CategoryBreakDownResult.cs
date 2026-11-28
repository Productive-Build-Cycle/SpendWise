using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Queries.Reports.GetCategoryBreakDown
{
    public sealed class CategoryBreakDownResult
    {
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = null!;
        public TransactionTypes Type { get; init; }
        public decimal TotalAmount { get; init; }

    }
}
