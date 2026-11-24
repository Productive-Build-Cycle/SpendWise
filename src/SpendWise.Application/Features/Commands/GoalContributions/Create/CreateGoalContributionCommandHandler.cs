using MediatR;
using SpendWise.Application.DTOs;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.ValueObjects;
using TransactionEntity = SpendWise.Domain.Entities.Transactions;
using SpendWise.Domain.Enums;
using SpendWise.Domain.Entities;

namespace SpendWise.Application.Features.Commands.GoalContributions.Create
{
    public sealed class CreateGoalContributionCommandHandler :
        IRequestHandler<CreateGoalContributionCommand, GoalContributionDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserLocalDateProvider _userLocalDateProvider;
        private readonly ICurrentUser _currentUser;
        private readonly IGoalContributionRepository _goalContributionRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly IFinancialGoalRepository _financialGoalRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public CreateGoalContributionCommandHandler(
            IUserRepository userRepositorry,
            IUserLocalDateProvider userLocalDateProvider,
            ICurrentUser currentUser,
            IGoalContributionRepository goalContributionRepository,
            IApplicationDbContext dbContext,
            IFinancialGoalRepository financialGoalRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _userRepository = userRepositorry;
            _userLocalDateProvider = userLocalDateProvider;
            _currentUser = currentUser;
            _goalContributionRepository = goalContributionRepository;
            _dbContext = dbContext;
            _financialGoalRepository = financialGoalRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<GoalContributionDto> Handle(
            CreateGoalContributionCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User was not found .");
            }
            var contributionLocalDate = _userLocalDateProvider.GetUserLocalDate(
                request.ContributionDate,
                user.TimeZoneId);

            var currentDate = _userLocalDateProvider.GetUserLocalDate(user.TimeZoneId);

            var financialGoal = await _financialGoalRepository.FindByIdForUserAsync(
                request.FinancialGoalId,
                userId,
                cancellationToken);
            if (financialGoal == null)
            {
                throw new NotFoundException("Goal was not found!");
            }

            var account = await _accountRepository.FindByIdForUserAsync(
                request.AccountId,
                userId,
                cancellationToken);
            if (account == null)
            {
                throw new NotFoundException("Account was not found !");
            }
            var sourceAccount = request.SourceAccountId.HasValue ?
                await _accountRepository.FindByIdForUserAsync(
                    request.SourceAccountId.Value,
                    userId,
                    cancellationToken)
                : null;
            if (request.SourceAccountId.HasValue && sourceAccount == null)
            {
                throw new NotFoundException("source account was not found");
            }

            var contributionAmount = new Money(
                request.Amount,
                new Currency("USD"));
            TransactionEntity? transferTransaction = null;
            if (sourceAccount is not null)
            {
                transferTransaction = new TransactionEntity(
                    sourceAccount.Id,
                    account.Id,
                    TransactionTypes.Transfer,
                    userId,
                    contributionAmount,
                    "Transfer for goal contribution",
                    request.ContributionDate.UtcDateTime,
                    null
                    );
                //sourcceAccount lose money account receivves money
                transferTransaction.ApplyToAccounts(sourceAccount, account);
                _transactionRepository.Add(transferTransaction);
            }

            var contribution = new GoalContribution(
                financialGoal.Id,
                contributionAmount,
                contributionLocalDate,
                currentDate,
                account.Id,
                transferTransaction?.Id
                );
            financialGoal.AddContribution(contribution);
            _goalContributionRepository.Add(contribution);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new GoalContributionDto(
                contribution.Id,
                contribution.FinancialGoalId,
                contribution.ContributionAmount.Amount,
                contribution.ContributionAmount.Currency.Code,
                contribution.ContributionDate,
                contribution.AccountId,
                contribution.TransactionId,
                contribution.RemainingAmount.Amount,
                Array.Empty<GoalContributionReleaseDto>()
                );
        }

    }
}
