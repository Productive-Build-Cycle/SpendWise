using SpendWise.Application.Common.Enums;

namespace SpendWise.Infrastructure.Authentication.RefreshTokens;

public sealed class RefreshTokenEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public Guid FamilyId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    public RefreshTokenRevocationReason? RevocationReason { get; private set; }

    // SQL Server maintains this value on every update.
    public byte[] RowVersion { get; private set; } = null!;

    private RefreshTokenEntity()
    {
    }

    public RefreshTokenEntity(Guid id, Guid userId, string tokenHash, Guid familyId, DateTime createdAtUtc,
        DateTime expiresAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Token ID cannot be empty.", nameof(id));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (familyId == Guid.Empty)
            throw new ArgumentException("Token family ID cannot be empty.", nameof(familyId));

        if (!IsValidHash(tokenHash))
            throw new ArgumentException("Token hash is invalid.", nameof(tokenHash));

        if (createdAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Creation time must be UTC.", nameof(createdAtUtc));

        if (expiresAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Expiration time must be UTC.", nameof(expiresAtUtc));

        if (expiresAtUtc <= createdAtUtc)
            throw new ArgumentException("Expiration must be after creation.", nameof(expiresAtUtc));

        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        FamilyId = familyId;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }


    public void Revoke(DateTime revokedAtUtc, RefreshTokenRevocationReason reason, Guid? replacedByTokenId = null)
    {
        if (revokedAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Revocation time must be UTC.", nameof(revokedAtUtc));

        if (RevokedAtUtc.HasValue)
            throw new InvalidOperationException("The refresh token is already revoked.");

        if (replacedByTokenId == Id)
            throw new ArgumentException("A refresh token cannot replace itself.", nameof(replacedByTokenId));

        RevokedAtUtc = revokedAtUtc;
        RevocationReason = reason;
        ReplacedByTokenId = replacedByTokenId;
    }

    private static bool IsValidHash(string? hash)
    {
        return hash is { Length: 64 } && hash.All(char.IsAsciiHexDigit);
    }
}