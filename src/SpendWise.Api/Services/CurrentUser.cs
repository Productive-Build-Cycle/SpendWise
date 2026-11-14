using System.IdentityModel.Tokens.Jwt;
using SpendWise.Application.Interfaces;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Api.Services;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var subject = _httpContextAccessor.HttpContext?
                .User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(subject, out var userId) || userId == Guid.Empty)
                throw new UnauthenticatedUserException();

            return userId;
        }
    }
}