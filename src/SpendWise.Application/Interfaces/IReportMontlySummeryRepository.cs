using SpendWise.Application.Features.Queries.Reports.GetMonthlySummery;


namespace SpendWise.Application.Interfaces
{
    public interface IReportMontlySummeryRepository
    {
        Task<IReadOnlyList<MontlySummeryResult>> GetMontlySummeryAsync(
            Guid userId,
            string timeZoneId,
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellationToken = default);
    }
}
