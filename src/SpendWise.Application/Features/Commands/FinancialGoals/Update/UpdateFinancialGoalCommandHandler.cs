using MediatR;
using SpendWise.Domain.ValueObjects;
using SpendWise.Application.Interfaces;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Application.Features.Commands.FinancialGoals.Update
{
    public sealed class UpdateFinancialGoalCommandHandler :
        IRequestHandler<UpdateFinancialGoalCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IFinancialGoalRepository _financialGoalRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IUserLocalDateProvider _userLocalDateProvider;
        public UpdateFinancialGoalCommandHandler(IApplicationDbContext dbContext,
            IFinancialGoalRepository financialGoalRepository,
            ICurrentUser currentUser,
            IUserRepository userRepository,
            IUserLocalDateProvider userLocalDateProvider)
        {
            _dbContext = dbContext;
            _financialGoalRepository = financialGoalRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _userLocalDateProvider = userLocalDateProvider;
        }
        public async Task<Unit>Handle(UpdateFinancialGoalCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId,cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User was not founded");
            }
            var currentDate = _userLocalDateProvider.GetUserLocalDate(user.TimeZoneId);

            var financialGoal = await _financialGoalRepository.FindByIdForUserAsync(
                request.GoalId, userId, cancellationToken);
            if (financialGoal == null)
            {
                throw new NotFoundException("FinancialGoal was not founded .");
            }
            financialGoal.Update(request.GoalName,
                new Money(request.TargetAmount,
                new Currency(request.CurrencyCode)),
                request.TargetDate, currentDate);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
