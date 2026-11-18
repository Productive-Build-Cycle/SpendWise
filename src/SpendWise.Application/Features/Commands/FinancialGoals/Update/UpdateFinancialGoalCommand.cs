using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Features.Commands.FinancialGoals.Update
{
    public sealed record UpdateFinancialGoalCommand(
        Guid GoalId,
        string GoalName,
        decimal TargetAmount,
        string CurrencyCode,
        DateOnly TargetDate) : IRequest<Unit>;

}
