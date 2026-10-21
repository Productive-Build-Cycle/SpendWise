namespace SpendWise.Infrastructure.Authentication.Jwt;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public int AccessTokenLifetimeMinutes { get; set; } = 15;

    public int ClockSkewSeconds { get; set; } = 30;

    public string ActiveKeyId { get; set; } = string.Empty;

    public string PrivateKeyPath { get; set; } = string.Empty;

    public List<JwtValidationKeyOptions> ValidationKeys { get; set; } = [];

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT Issuer is required.");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT Audience is required.");

        if (AccessTokenLifetimeMinutes is < 1 or > 15)
            throw new InvalidOperationException("Access token lifetime must be between 1 and 15 minutes.");

        if (ClockSkewSeconds is < 0 or > 60)
            throw new InvalidOperationException("JWT clock skew must be between 0 and 60 seconds.");

        if (!IsValidKeyId(ActiveKeyId))
            throw new InvalidOperationException("JWT ActiveKeyId is invalid.");

        if (string.IsNullOrWhiteSpace(PrivateKeyPath) ||
            !Path.IsPathFullyQualified(PrivateKeyPath) ||
            !File.Exists(PrivateKeyPath))
        {
            throw new InvalidOperationException(
                "The JWT private key file is missing or its path is invalid.");
        }

        if (ValidationKeys is null || ValidationKeys.Count == 0)
            throw new InvalidOperationException("At least one JWT validation key is required.");

        var duplicateKeyIds = ValidationKeys
            .GroupBy(key => key.KeyId, StringComparer.Ordinal)
            .Any(group => group.Count() > 1);

        if (duplicateKeyIds)
            throw new InvalidOperationException("JWT validation key IDs must be unique.");

        foreach (var key in ValidationKeys)
        {
            if (!IsValidKeyId(key.KeyId))
                throw new InvalidOperationException("A JWT validation key ID is invalid.");

            if (string.IsNullOrWhiteSpace(key.PublicKeyPath) ||
                !Path.IsPathFullyQualified(key.PublicKeyPath) ||
                !File.Exists(key.PublicKeyPath))
            {
                throw new InvalidOperationException($"JWT public key file for '{key.KeyId}' is missing or invalid.");
            }
        }

        if (!ValidationKeys.Any(key => string.Equals(key.KeyId, ActiveKeyId, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("The active signing key must also be present in validation keys.");
        }
    }

    private static bool IsValidKeyId(string? keyId)
    {
        return !string.IsNullOrWhiteSpace(keyId)
               && keyId.Length <= 64
               && keyId.All(character =>
                   char.IsAsciiLetterOrDigit(character)
                   || character is '-' or '_' or '.');
    }
}

public sealed class JwtValidationKeyOptions
{
    public string KeyId { get; set; } = string.Empty;

    public string PublicKeyPath { get; set; } = string.Empty;
}