using FluentValidation;

namespace SpendWise.Application.Features.Commands.Accounts.Update;

public sealed class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();

        RuleFor(x => x.AccountName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CreditLimit)
            .Must(limit => !limit.HasValue || limit.Value > 0)
            .WithMessage("Credit limit must be greater than zero.");
    }
}