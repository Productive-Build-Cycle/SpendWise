
using MediatR;

namespace SpendWise.Application.Features.Queries.Reports.GetMonthlySummery
{
    public sealed record GetMontlySummeryQuery(
        int year) : IRequest<IReadOnlyList<MontlySummeryResult>>;

}
