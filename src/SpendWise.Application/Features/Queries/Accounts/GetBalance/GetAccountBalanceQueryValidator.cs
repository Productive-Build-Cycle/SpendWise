using FluentValidation;

namespace SpendWise.Application.Features.Queries.Accounts.GetBalance;

public sealed class GetAccountBalanceQueryValidator : AbstractValidator<GetAccountBalanceQuery>
{
    public GetAccountBalanceQueryValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}