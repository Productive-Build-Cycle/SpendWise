using Microsoft.IdentityModel.Tokens;

namespace SpendWise.Infrastructure.Authentication.Jwt;

public interface IJwtKeyProvider
{
    SecurityKey SigningKey { get; }

    IReadOnlyCollection<SecurityKey> ValidationKeys { get; }
}