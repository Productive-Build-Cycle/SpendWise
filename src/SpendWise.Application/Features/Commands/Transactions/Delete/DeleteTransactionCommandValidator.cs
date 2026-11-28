using FluentValidation;

namespace SpendWise.Application.Features.Commands.Transactions.Delete;

public sealed class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}