using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.DTOs
{
    public sealed record GoalContributionReleaseDto(
        Guid Id,
        Guid FinancialGoalId,
        string GoalName,
        Guid GoalContributionId,
        decimal ReleaseAmount,
        DateOnly ReleaseDate,
        Guid AccountId,
        Guid? TransactionId
        );

}
