using System.Security.Cryptography;
using System.Text;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;

namespace SpendWise.Infrastructure.Authentication.RefreshTokens;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private const int TokenByteLength = 32;
    private const int TokenTextLength = 43;

    public RefreshTokenMaterial Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenByteLength);

        try
        {
            var rawToken = Convert.ToBase64String(randomBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            var tokenHash = Hash(rawToken);

            return new RefreshTokenMaterial(rawToken, tokenHash);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(randomBytes);
        }
    }

    public string Hash(string rawToken)
    {
        ArgumentNullException.ThrowIfNull(rawToken);

        if (rawToken.Length != TokenTextLength || rawToken.Any(character =>
                !(char.IsAsciiLetterOrDigit(character) || character is '-' or '_')))
        {
            throw new ArgumentException("The refresh token format is invalid.", nameof(rawToken));
        }

        var tokenBytes = Encoding.ASCII.GetBytes(rawToken);

        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}