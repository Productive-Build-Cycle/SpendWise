using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Interfaces;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Domain.Entities;
using GoalContributionReleaseEntity = SpendWise.Domain.Entities.GoalContributionRelease;
using SpendWise.Domain.ValueObjects;
using TransactionEntity = SpendWise.Domain.Entities.Transactions;
using SpendWise.Domain.Enums;
using SpendWise.Application.Services;


namespace SpendWise.Application.Features.Commands.GoalContributionRelease.Create
{
    public sealed class CreateGoalContributionReleaseCommandHandler :
        IRequestHandler<CreateGoalContributionReleaseCommand,
            GoalContributionReleaseDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IGoalContributionRepository _goalContributionRepository;
        private readonly IUserLocalDateProvider _userLocalDateProvide;
        private readonly IGoalContributionReleaseService _releaseService;

        public CreateGoalContributionReleaseCommandHandler(
            IApplicationDbContext dbContext,
            ICurrentUser currentUser,
            IUserRepository userRepository,
            IUserLocalDateProvider userLocalDateProvide,
            IGoalContributionRepository goalContributionRepository,
            IGoalContributionReleaseService releaseService

            )
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _userLocalDateProvide = userLocalDateProvide;
            _goalContributionRepository = goalContributionRepository;
            _releaseService = releaseService;

        }
        public async Task<GoalContributionReleaseDto> Handle(
            CreateGoalContributionReleaseCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId,
                cancellationToken);
            if (user is null)
            {
                throw new NotFoundException("user Not Found .");
            }
            var releaseLocalDate = _userLocalDateProvide.GetUserLocalDate(
                request.ReleaseDate, user.TimeZoneId);

            var currentDate = _userLocalDateProvide.GetUserLocalDate(
                user.TimeZoneId);

            GoalContribution? contribution = await _goalContributionRepository.
                FindByIdForUserAsync(request.GoalContributionId,
                userId,
                cancellationToken);


            if (contribution is null)
            {
                throw new NotFoundException("Goal Contribution not gound .");
            }

            var releaseAmount = new Money(
                request.ReleaseAmount,
                new Currency("USD"));
            var release = await _releaseService.CreateReleaseAsync(
                contribution,
                releaseAmount,
                releaseLocalDate,
                currentDate,
                request.ReleaseDate.UtcDateTime,
                userId,
                cancellationToken
                );
            await _dbContext.SaveChangesAsync();
            return new GoalContributionReleaseDto(
                release.Id,
                contribution.FinancialGoalId,
                contribution.FinancialGoal.GoalName,
                release.GoalContributionId,
                release.ReleaseAmount.Amount,
                release.ReleaseDate,
                contribution.AccountId,
                release.TransactionId
                );
        }
    }
}
