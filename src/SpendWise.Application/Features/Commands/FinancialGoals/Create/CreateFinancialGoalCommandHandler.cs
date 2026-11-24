using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;


namespace SpendWise.Application.Features.Commands.FinancialGoals.Create
{
    public sealed class CreateFinancialGoalCommandHandler : IRequestHandler<CreateFinancialGoalCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUserLocalDateProvider _userDateProvider;
        private readonly IApplicationDbContext _dbContext;
        private readonly IFinancialGoalRepository _financialGoalRepository;

        public CreateFinancialGoalCommandHandler(IUserRepository userRepository,
            ICurrentUser currentUser,
            IUserLocalDateProvider userDateProvider,
            IApplicationDbContext dbContext,
            IFinancialGoalRepository financialGoalRepository)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _userDateProvider = userDateProvider;
            _dbContext = dbContext;
            _financialGoalRepository = financialGoalRepository;
        }
        public async Task<Guid> Handle(CreateFinancialGoalCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User  was not found .");
            }
            var currentDate = _userDateProvider.GetUserLocalDate(user.TimeZoneId);

            var currency = new Currency(request.CurrencyCode);

            var targetAmount = new Money(request.TargetAmount, currency);

            var financialGoal = new FinancialGoal(userId,
                request.GoalName, targetAmount, request.TargetDate, currentDate);

            _financialGoalRepository.Add(financialGoal);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return financialGoal.Id;
        }
    }
}
