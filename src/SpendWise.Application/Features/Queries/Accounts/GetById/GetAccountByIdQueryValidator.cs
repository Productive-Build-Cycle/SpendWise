using FluentValidation;

namespace SpendWise.Application.Features.Queries.Accounts.GetById;

public sealed class GetAccountByIdQueryValidator : AbstractValidator<GetAccountByIdQuery>
{
    public GetAccountByIdQueryValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}