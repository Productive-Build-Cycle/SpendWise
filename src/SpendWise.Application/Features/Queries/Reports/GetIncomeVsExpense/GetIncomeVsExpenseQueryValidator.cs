using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpendWise.Application.Features.Queries.Reports.GetIncomeVsExpense
{
    public sealed class GetIncomeVsExpenseQueryValidator :
        AbstractValidator<GetIncomeVsExpenseQuery>
    {
        public GetIncomeVsExpenseQueryValidator()
        {
            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate)
                .WithMessage("From date must bee less than or equal to date");
        }
    }
}
