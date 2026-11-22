using FluentValidation;

namespace SpendWise.Application.Features.Commands.GoalContributions.Create
{
    public sealed class CreateGoalContributionCommandValidator :
        AbstractValidator<CreateGoalContributionCommand>
    {
        public CreateGoalContributionCommandValidator()
        {
            RuleFor(x => x.FinancialGoalId)
                .NotEmpty()
                .WithMessage("Financial goal Id is required.");
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithMessage("Account Id is required .");
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Contribution must be greather than zero.");
            RuleFor(x => x.SourceAccountId)
                .NotEmpty()
                .When(x => x.SourceAccountId.HasValue)
                .WithMessage("Source Account Id can not be empty");
            RuleFor(x => x)
                .Must(x => !x.SourceAccountId.HasValue ||
                x.SourceAccountId.Value != x.AccountId)
                .WithMessage("Source account and destination account can not be the same");
        }
    }
}
