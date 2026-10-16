using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace SpendWise.Infrastructure.Authentication.Jwt;

public sealed class JwtService : IJwtService
{
    private readonly JwtOptions _options;
    private readonly IJwtKeyProvider _keyProvider;

    public JwtService(JwtOptions options, IJwtKeyProvider keyProvider)
    {
        _options = options;
        _keyProvider = keyProvider;
    }

    public AccessTokenResult CreateAccessToken(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        var now = DateTime.UtcNow;
        var issuedAtUnix = new DateTimeOffset(now).ToUnixTimeSeconds();

        var expiresAt = now.AddMinutes(_options.AccessTokenLifetimeMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString("D")),

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var credentials = new SigningCredentials(_keyProvider.SigningKey, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        token.Payload[JwtRegisteredClaimNames.Iat] = issuedAtUnix;

        token.Header["kid"] = _options.ActiveKeyId;
        token.Header["typ"] = "JWT";

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(tokenValue, expiresAt, _options.AccessTokenLifetimeMinutes * 60);
    }
}
