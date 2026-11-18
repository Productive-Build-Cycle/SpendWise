using MediatR;
using SpendWise.Application.DTOs;


namespace SpendWise.Application.Features.Queries.FinancialGoals.GetById
{
    public sealed record GetFinancialGoalByIdQuery(Guid GoalId) :
        IRequest<FinancialGoalDto>;

}
