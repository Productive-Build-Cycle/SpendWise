using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;
using SpendWise.Infrastructure.Persistence;

namespace SpendWise.Infrastructure.Authentication.RefreshTokens;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(RefreshTokenRecord refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        if (refreshToken.RevokedAtUtc.HasValue || refreshToken.ReplacedByTokenId.HasValue ||
            refreshToken.RevocationReason.HasValue)
        {
            throw new ArgumentException("A new refresh token cannot already be revoked or replaced.",
                nameof(refreshToken));
        }

        var entity = new RefreshTokenEntity(
            refreshToken.Id,
            refreshToken.UserId,
            refreshToken.TokenHash,
            refreshToken.FamilyId,
            refreshToken.CreatedAtUtc,
            refreshToken.ExpiresAtUtc);

        _dbContext.RefreshTokens.Add(entity);
    }

    public async Task<RefreshTokenRecord?> FindByTokenHashAsync(string tokenHash,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidHash(tokenHash))
            return null;

        var entity = await _dbContext.RefreshTokens
            .AsNoTracking()
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

        if (entity is null)
            return null;

        // SQL Server datetime2 does not preserve DateTime.Kind.
        // Our database convention is that these values are stored as UTC.
        return new RefreshTokenRecord(
            entity.Id,
            entity.UserId,
            entity.TokenHash,
            entity.FamilyId,
            DateTime.SpecifyKind(entity.CreatedAtUtc, DateTimeKind.Utc),
            DateTime.SpecifyKind(entity.ExpiresAtUtc, DateTimeKind.Utc),
            entity.RevokedAtUtc.HasValue ? DateTime.SpecifyKind(entity.RevokedAtUtc.Value, DateTimeKind.Utc) : null,
            entity.ReplacedByTokenId,
            entity.RevocationReason);
    }

    private static bool IsValidHash(string? hash)
    {
        return hash is { Length: 64 } && hash.All(char.IsAsciiHexDigit);
    }
}