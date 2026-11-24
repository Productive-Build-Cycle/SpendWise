using Microsoft.IdentityModel.Tokens;
using SpendWise.Domain.Entities;
using SpendWise.Domain.ValueObjects;

namespace SpendWise.Application.Interfaces
{
    public interface IGoalContributionReleaseService
    {
        Task<GoalContributionRelease> CreateReleaseAsync(
            GoalContribution contribution,
            Money releaseAmount,
            DateOnly releaseDate,
            DateOnly currentDate,
            DateTime transactionDateUtc,
            Guid uderId,
            CancellationToken cancellationToken = default
            );

    }
}
