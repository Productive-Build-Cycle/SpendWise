using MediatR;
using SpendWise.Application.Interfaces;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense
{
    public sealed class GetIncomeVsExpenseQueryHandler :
        IRequestHandler<GetIncomeVsExpenseQuery, IncomeVsExpenseResult>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUserLocalDateToUtcConverter _userLocalDateToUtc;

        public GetIncomeVsExpenseQueryHandler(IReportRepository reportRepository,
            IUserRepository userRepository,
            ICurrentUser currentUser,
            IUserLocalDateToUtcConverter userLocalDateToUtc)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
            _userLocalDateToUtc = userLocalDateToUtc;
        }
        public async Task<IncomeVsExpenseResult> Handle(GetIncomeVsExpenseQuery request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException("User not found !");
            }
            var fromDate = _userLocalDateToUtc.ConvertLocalDateToUtc(
                request.FromDate, user.TimeZoneId);
            var toDate = _userLocalDateToUtc.ConvertLocalDateToUtc(
                request.ToDate.AddDays(1), user.TimeZoneId);

            return await _reportRepository.GetIncomeVsExpenseAsync(
                userId,
                fromDate,
                toDate,
                cancellationToken
                );


        }


    }
}
