using SpendWise.Application.Common.Enums;
using SpendWise.Application.Common.Models;

namespace SpendWise.Application.Interfaces;

public interface IRefreshTokenRepository
{
    void Add(RefreshTokenRecord refreshToken);

    Task<RefreshTokenRecord?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task RevokeAsync(Guid tokenId, DateTime revokedAtUtc, RefreshTokenRevocationReason reason,
        Guid? replacedByTokenId = null, CancellationToken cancellationToken = default);

    Task RevokeActiveFamilyAsync(Guid familyId, DateTime revokedAtUtc, RefreshTokenRevocationReason reason,
        CancellationToken cancellationToken = default);
}