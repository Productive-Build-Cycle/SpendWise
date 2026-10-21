using SpendWise.Application.Common.Enums;

namespace SpendWise.Application.Common.Models;

public sealed record RefreshTokenRecord(
    Guid Id,
    Guid UserId,
    string TokenHash,
    Guid FamilyId,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc,
    DateTime? RevokedAtUtc = null,
    Guid? ReplacedByTokenId = null,
    RefreshTokenRevocationReason? RevocationReason = null);