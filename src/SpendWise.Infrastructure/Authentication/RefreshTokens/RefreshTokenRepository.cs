using Microsoft.EntityFrameworkCore;
using SpendWise.Application.Common.Enums;
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

        return new RefreshTokenRecord(
            entity.Id,
            entity.UserId,
            entity.TokenHash,
            entity.FamilyId,
            entity.CreatedAtUtc,
            entity.ExpiresAtUtc,
            entity.RevokedAtUtc,
            entity.ReplacedByTokenId,
            entity.RevocationReason);
    }

    public async Task RevokeAsync(Guid tokenId, DateTime revokedAtUtc, RefreshTokenRevocationReason reason,
        Guid? replacedByTokenId = null, CancellationToken cancellationToken = default)
    {
        if (tokenId == Guid.Empty)
            throw new ArgumentException("Token ID cannot be empty.", nameof(tokenId));

        if (revokedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Revocation time must be UTC.", nameof(revokedAtUtc));
        }

        var entity = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.Id == tokenId, cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Refresh token was not found.");

        entity.Revoke(revokedAtUtc, reason, replacedByTokenId);
    }

    public async Task RevokeActiveFamilyAsync(Guid familyId, DateTime revokedAtUtc, RefreshTokenRevocationReason reason,
        CancellationToken cancellationToken = default)
    {
        if (familyId == Guid.Empty)
            throw new ArgumentException("Token family ID cannot be empty.", nameof(familyId));

        if (revokedAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Revocation time must be UTC.", nameof(revokedAtUtc));

        var activeTokens = await _dbContext.RefreshTokens
            .Where(token => token.FamilyId == familyId && token.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke(revokedAtUtc, reason);
        }
    }

    private static bool IsValidHash(string? hash)
    {
        return hash is { Length: 64 } && hash.All(char.IsAsciiHexDigit);
    }
}