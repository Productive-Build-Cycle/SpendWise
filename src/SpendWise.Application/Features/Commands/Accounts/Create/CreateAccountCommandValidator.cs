using FluentValidation;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Commands.Accounts.Create;

public sealed class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.AccountName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AccountType)
            .IsInEnum();

        RuleFor(x => x.OpeningBalance)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CreditLimit)
            .Must((command, limit) =>
                command.AccountType != AccountType.CreditCard || limit.HasValue)
            .WithMessage("Credit limit is required for credit card accounts.")
            .Must((command, limit) =>
                command.AccountType == AccountType.CreditCard || !limit.HasValue)
            .WithMessage("Credit limit is only allowed for credit card accounts.")
            .Must(limit => !limit.HasValue || limit.Value > 0)
            .WithMessage("Credit limit must be greater than zero.")
            .Must((command, limit) =>
                !limit.HasValue || limit.Value >= command.OpeningBalance)
            .WithMessage("Opening balance cannot exceed credit limit.");
    }
}