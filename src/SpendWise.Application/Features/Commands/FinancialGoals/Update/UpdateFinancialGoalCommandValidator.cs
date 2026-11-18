using FluentValidation;

namespace SpendWise.Application.Features.Commands.FinancialGoals.Update
{
    public sealed class UpdateFinancialGoalCommandValidator :
        AbstractValidator<UpdateFinancialGoalCommand>
    {
        public UpdateFinancialGoalCommandValidator()
        {
            RuleFor(x => x.GoalId)
                .NotEmpty();
            RuleFor(x => x.GoalName)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.TargetAmount)
                .GreaterThan(0);
            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .Matches("^[A-Za-z]{3}$");

        }
    }
}
