using FluentValidation;
using SpendWise.Domain.Enums;

namespace SpendWise.Application.Features.Commands.Categories.Create;

public sealed class CreateCategoryCommandValidator
    : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CategoryType)
            .Must(type => !string.IsNullOrWhiteSpace(type) && Enum.GetNames<CategoryType>()
                .Contains(type.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid category type.");

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}