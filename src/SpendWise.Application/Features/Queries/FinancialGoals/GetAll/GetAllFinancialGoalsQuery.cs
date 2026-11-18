using MediatR;
using SpendWise.Application.DTOs;

namespace SpendWise.Application.Features.Queries.FinancialGoals.GetAll
{
    public sealed record GetAllFinancialGoalsQuery():
        IRequest<IReadOnlyList<FinancialGoalDto>>
    {

    }
}
