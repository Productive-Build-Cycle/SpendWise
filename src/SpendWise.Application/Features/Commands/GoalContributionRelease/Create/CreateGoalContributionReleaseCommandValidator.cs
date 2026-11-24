using FluentValidation;


namespace SpendWise.Application.Features.Commands.GoalContributionRelease.Create
{
    public sealed class CreateGoalContributionReleaseCommandValidator :
        AbstractValidator<CreateGoalContributionReleaseCommand>
    {
        public CreateGoalContributionReleaseCommandValidator()
        {
            RuleFor(x => x.GoalContributionId)
                .NotEmpty()
                .WithMessage("Goal contribution ID is required.");

            RuleFor(x => x.ReleaseAmount)
                .GreaterThan(0)
                .WithMessage("Release Amount must be greather than zero");

        }
    }
}
