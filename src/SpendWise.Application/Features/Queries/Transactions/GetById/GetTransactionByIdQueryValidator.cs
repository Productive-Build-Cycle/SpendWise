using FluentValidation;

namespace SpendWise.Application.Features.Queries.Transactions.GetById;

public sealed class GetTransactionByIdQueryValidator : AbstractValidator<GetTransactionByIdQuery>
{
    public GetTransactionByIdQueryValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}
