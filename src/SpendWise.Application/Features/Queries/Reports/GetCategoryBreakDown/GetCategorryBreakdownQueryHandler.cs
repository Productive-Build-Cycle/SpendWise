using MediatR;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Enums;


namespace SpendWise.Application.Features.Queries.Reports.GetCategoryBreakDown
{
    public sealed class GetCategorryBreakdownQueryHandler
        : IRequestHandler<GetCategoryBreakDownQuesry, IReadOnlyList<CategoryBreakDownResult>>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IReportRepository _reportRepository;
        private readonly IUserLocalDateToUtcConverter _dateConvertor;

        private readonly IUserRepository _userRepository;
        public GetCategorryBreakdownQueryHandler(
            ICurrentUser currentUser,
            IReportRepository reportRepository,
            IUserLocalDateToUtcConverter dateConvertor,
            IUserRepository userRepository
            )
        {
            _currentUser = currentUser;
            _reportRepository = reportRepository;
            _dateConvertor = dateConvertor;
            _userRepository = userRepository;
        }
        public async Task<IReadOnlyList<CategoryBreakDownResult>> Handle
            (GetCategoryBreakDownQuesry request,
            CancellationToken cancellationToken
            )
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);

            var fromUtc = _dateConvertor.ConvertLocalDateToUtc(
                request.fromDate, user.TimeZoneId);

            var toUtc = _dateConvertor.ConvertLocalDateToUtc(
                request.toDate.AddDays(1), user.TimeZoneId);

            return await _reportRepository.GetCategoryBreakdownAsync(
                userId,
                fromUtc,
                toUtc,
                request.categoryId,
                request.transactionType,
                cancellationToken
                );
        }
    }
}
