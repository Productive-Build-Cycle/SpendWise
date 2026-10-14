using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SpendWise.Infrastructure.Authentication.Jwt;

namespace SpendWise.Api.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<JwtOptions, IJwtKeyProvider>((options, jwt, keyProvider) =>
            {
                options.MapInboundClaims = false;
                options.IncludeErrorDetails = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,

                        RequireSignedTokens = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = keyProvider.ValidationKeys,

                        ValidAlgorithms = [SecurityAlgorithms.RsaSha256],

                        ValidTypes = ["JWT"],

                        ClockSkew = TimeSpan.FromSeconds(jwt.ClockSkewSeconds),

                        RoleClaimType = "role"
                    };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var principal = context.Principal;

                        var subject = principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                        var jti = principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                        var iat = principal?.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;

                        var validSubject = Guid.TryParse(subject, out var userId) && userId != Guid.Empty;

                        var validJti = Guid.TryParseExact(jti, "N", out _);

                        var validIssuedAt =
                            long.TryParse(iat, NumberStyles.None, CultureInfo.InvariantCulture, out var issuedAt)
                            && issuedAt <= DateTimeOffset.UtcNow.ToUnixTimeSeconds() + jwt.ClockSkewSeconds;

                        if (!validSubject || !validJti || !validIssuedAt)
                        {
                            context.Fail("The access token is invalid.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        return services;
    }
}