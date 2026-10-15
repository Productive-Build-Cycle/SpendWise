using FluentValidation;

namespace SpendWise.Application.Features.Commands.Authentication.Refresh;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RawRefreshToken).NotEmpty();
    }
}