using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;
using SpendWise.Domain.Enums;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Application.Services
{
    public class GoalContributionReleaseService : IGoalContributionReleaseService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IGoalContributionReleaseRepository _releaseRepository;
        private readonly IAccountRepository _accountRepository;
        public GoalContributionReleaseService(
            ITransactionRepository transactionRepository,
            IGoalContributionReleaseRepository releaseRepository,
            IAccountRepository accountRepository)
        {
            _transactionRepository = transactionRepository;
            _releaseRepository = releaseRepository;
            _accountRepository = accountRepository;
        }
        public async Task<GoalContributionRelease> CreateReleaseAsync(
            GoalContribution contribution,
            Money releaseAmount,
            DateOnly releaseDate,
            DateOnly currentDate,
            DateTime transactionDateUtc,
            Guid userId,
            CancellationToken cancellationToken = default
            )
        {
            Transactions? reverseTransfer = null;

            if (contribution.TransactionId.HasValue)
            {
                var originalTransaction =
                    await _transactionRepository.FindByIdForUserReadOnlyAsync(
                        contribution.TransactionId.Value,
                        userId,
                        cancellationToken);
                if (originalTransaction is null)
                {
                    throw new NotFoundException("Original contribution transaction  not found .");
                }

                if (originalTransaction.TransactionType != TransactionTypes.Transfer ||
                    originalTransaction.SourceAccountId is null ||
                    originalTransaction.DestinationAccountId != contribution.AccountId)
                {
                    throw new NotFoundException(
                        "Original contribution transaction is not a valid transfer.");
                }

                var holdingAccount = await _accountRepository.FindByIdForUserAsync(
                    contribution.AccountId,
                    userId,
                    cancellationToken
                    );
                if (holdingAccount is null)
                {
                    throw new NotFoundException("Goal contribution account not found");
                }

                var originalFundingAccount =
                    await _accountRepository.FindByIdForUserAsync(
                        originalTransaction.SourceAccountId.Value,
                        userId,
                        cancellationToken
                        );
                if (originalFundingAccount is null)
                {
                    throw new NotFoundException("Original funding account not found");
                }

                reverseTransfer = new Transactions(
                   holdingAccount.Id,
                   originalFundingAccount.Id,
                   TransactionTypes.Transfer,
                   userId,
                   releaseAmount,
                   "Transfer for goal contribution release",
                   transactionDateUtc,
                   null
                   );
                reverseTransfer.ApplyToAccounts(
                    holdingAccount,
                    originalFundingAccount);

                _transactionRepository.Add(reverseTransfer);
            }
            var release = new GoalContributionRelease(
                contribution.Id,
                releaseAmount,
                releaseDate,
                currentDate,
                reverseTransfer?.Id
                );
            contribution.AddRelease(release, currentDate);
            _releaseRepository.Add(release);
            return release;

        }

    }
}
