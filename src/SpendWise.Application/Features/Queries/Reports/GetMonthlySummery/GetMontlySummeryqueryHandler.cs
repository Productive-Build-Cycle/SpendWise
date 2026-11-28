using MediatR;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.ValueObjects;
using SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense;

namespace SpendWise.Application.Features.Queries.Reports.GetMonthlySummery
{
    public class GetMontlySummeryqueryHandler : IRequestHandler
        <GetMontlySummeryQuery, IReadOnlyList<MontlySummeryResult>>
    {
        private readonly IUserLocalDateToUtcConverter _dateConvertor;
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;

        private readonly IReportMontlySummeryRepository _reportRepository;

        public GetMontlySummeryqueryHandler(
            IUserLocalDateToUtcConverter dateConvertor,
            ICurrentUser currentUser,
            IUserRepository userRepository,
            IReportMontlySummeryRepository reportRepository)
        {
            _dateConvertor = dateConvertor;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _reportRepository = reportRepository;
        }
        public async Task<IReadOnlyList<MontlySummeryResult>> Handle(
            GetMontlySummeryQuery query,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            var fromDate = new DateOnly(query.year, 1, 1);
            var toDate = new DateOnly(query.year + 1, 1, 1);
            var fromUtc = _dateConvertor.ConvertLocalDateToUtc(fromDate,
                user.TimeZoneId);
            var toUtc = _dateConvertor.ConvertLocalDateToUtc(toDate,
                user.TimeZoneId);
            var monthlyResult = await _reportRepository.GetMontlySummeryAsync(
                 userId,
                 user.TimeZoneId,
                 fromUtc,
                 toUtc,
                 cancellationToken);
            var result = new List<MontlySummeryResult>();
            for (int month = 1; month <= 12; month++)
            {
                var existingMonth = monthlyResult
                    .FirstOrDefault(m => m.month == month);
                if (existingMonth != null)
                {
                    result.Add(existingMonth);
                }
                else
                {
                    result.Add(new MontlySummeryResult(
                        month,
                        new IncomeVsExpenseResult
                        {
                            TotalIncome = new Money(0m, new Currency("USD")),
                            TotalExpense = new Money(0m, new Currency("USD"))
                        }
                        ));
                }
            }
            return result;
        }
    }
}
