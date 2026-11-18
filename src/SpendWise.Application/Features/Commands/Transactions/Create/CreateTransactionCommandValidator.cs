using FluentValidation;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Commands.Transactions.Create;

public sealed class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionType)
            .Must(type =>
                !string.IsNullOrWhiteSpace(type) &&
                Enum.GetNames<TransactionTypes>()
                    .Contains(type.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid transaction type.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .Must(amount => decimal.Round(amount, 2) == amount)
            .WithMessage("Amount cannot have more than two decimal places.");

        RuleFor(x => x.SourceAccountId)
            .NotEqual(Guid.Empty)
            .When(x => x.SourceAccountId.HasValue);

        RuleFor(x => x.DestinationAccountId)
            .NotEqual(Guid.Empty)
            .When(x => x.DestinationAccountId.HasValue);

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
