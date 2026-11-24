using FluentValidation;

namespace SpendWise.Application.Features.Queries.FinancialGoals.GetById
{
    public sealed class GetFinancialGoalByIdQueryValidator : AbstractValidator<GetFinancialGoalByIdQuery>
    {
        public GetFinancialGoalByIdQueryValidator()
        {
            RuleFor(x => x.GoalId)
                .NotEmpty();
        }
    }
}
