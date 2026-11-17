using MediatR;

namespace SpendWise.Application.Features.Commands.FinancialGoals.Create
{
    public sealed record CreateFinancialGoalCommand(string GoalName,
        decimal TargetAmount,
        string CurrencyCode,
        DateOnly TargetDate) :
        IRequest<Guid>;

}
