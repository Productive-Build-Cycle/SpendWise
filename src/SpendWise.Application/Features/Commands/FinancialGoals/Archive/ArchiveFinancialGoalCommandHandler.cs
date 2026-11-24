using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Application.Services;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Commands.FinancialGoals.Archive
{
    public sealed class ArchiveFinancialGoalCommandHandler :
        IRequestHandler<ArchiveFinancialGoalCommand, Unit>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IFinancialGoalRepository _financialGoalRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserLocalDateProvider _userLocalDateProvider;
        private readonly IGoalContributionReleaseService _releaseService;

        public ArchiveFinancialGoalCommandHandler(
            ICurrentUser currentUser,
            IFinancialGoalRepository financialGoalRepository,
            IApplicationDbContext dbContext,
            IUserRepository userRepository,
            IUserLocalDateProvider localDateProvider,
            IGoalContributionReleaseService goalContributionReleaseService)
        {
            _currentUser = currentUser;
            _financialGoalRepository = financialGoalRepository;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _userLocalDateProvider = localDateProvider;
            _releaseService = goalContributionReleaseService;
        }
        public async Task<Unit> Handle(ArchiveFinancialGoalCommand request,
            CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException("User not found.");
            }

            var userCurrentDate = _userLocalDateProvider.GetUserLocalDate(
                user.TimeZoneId);

            var transactionDateTime = DateTime.UtcNow;

            var financialGoal = await _financialGoalRepository.FindByIdForUserAsync(
                request.FinancialGoalId,
                userId,
                cancellationToken);
            if (financialGoal is null)
            {
                throw new NotFoundException("Financial goal not found .");
            }

            foreach (var contribution in financialGoal.Contributions)
            {
                var remainingAmount = contribution.RemainingAmount;
                if (remainingAmount.Amount <= 0)
                {
                    continue;
                }
                await _releaseService.CreateReleaseAsync(
                    contribution,
                    remainingAmount,
                    userCurrentDate,
                    userCurrentDate,
                    transactionDateTime,
                    userId,
                    cancellationToken
                    );
            }
            financialGoal.Archive();

            await _dbContext.SaveChangesAsync();

            return Unit.Value;

        }
    }
}
