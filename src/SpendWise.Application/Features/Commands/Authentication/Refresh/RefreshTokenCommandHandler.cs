using MediatR;
using SpendWise.Application.Common.Enums;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;


namespace SpendWise.Application.Features.Commands.Authentication.Refresh;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITransactionManager _transactionManager;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IJwtService jwtService,
        IApplicationDbContext dbContext,
        ITransactionManager transactionManager)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _jwtService = jwtService;
        _dbContext = dbContext;
        _transactionManager = transactionManager;
    }

    public async Task<RefreshTokenCommandResult> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        string tokenHash;

        try
        {
            tokenHash = _refreshTokenService.Hash(request.RawRefreshToken);
        }
        catch (ArgumentException)
        {
            return new RefreshTokenCommandResult(false);
        }

        var currentToken = await _refreshTokenRepository.FindByTokenHashAsync(tokenHash, cancellationToken);

        if (currentToken is null)
            return new RefreshTokenCommandResult(false);

        // A previously rotated token was reused.
        if (currentToken.RevokedAtUtc.HasValue)
        {
            var wasRotated = currentToken.RevocationReason == RefreshTokenRevocationReason.Rotation ||
                             currentToken.ReplacedByTokenId.HasValue;

            if (wasRotated)
            {
                await RevokeFamilyAsync(currentToken.FamilyId, now, cancellationToken);
            }

            return new RefreshTokenCommandResult(false);
        }

        if (currentToken.ExpiresAtUtc <= now)
            return new RefreshTokenCommandResult(false);

        var newTokenMaterial = _refreshTokenService.Generate();
        var newTokenId = Guid.NewGuid();
        var newTokenExpiresAtUtc = now.Add(_refreshTokenService.Lifetime);

        var newToken = new RefreshTokenRecord(
            Id: newTokenId,
            UserId: currentToken.UserId,
            TokenHash: newTokenMaterial.TokenHash,
            FamilyId: currentToken.FamilyId,
            CreatedAtUtc: now,
            ExpiresAtUtc: newTokenExpiresAtUtc);

        // Generate the access token before modifying the database.
        var accessToken = _jwtService.CreateAccessToken(currentToken.UserId);

        try
        {
            await _transactionManager.ExecuteAsync(async ct =>
            {
                await _refreshTokenRepository.RevokeAsync(
                    currentToken.Id,
                    now,
                    RefreshTokenRevocationReason.Rotation,
                    newTokenId,
                    ct);

                _refreshTokenRepository.Add(newToken);

                await _dbContext.SaveChangesAsync(ct);
            }, cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            var latestToken = await _refreshTokenRepository.FindByTokenHashAsync(tokenHash, cancellationToken);

            if (latestToken is not null &&
                latestToken.RevokedAtUtc.HasValue &&
                (latestToken.RevocationReason == RefreshTokenRevocationReason.Rotation ||
                 latestToken.ReplacedByTokenId.HasValue))
            {
                await RevokeFamilyAsync(latestToken.FamilyId, DateTime.UtcNow, cancellationToken);
            }

            return new RefreshTokenCommandResult(false);
        }

        return new RefreshTokenCommandResult(
            Succeeded: true,
            AccessToken: accessToken.Token,
            ExpiresInSeconds: accessToken.ExpiresInSeconds,
            RawRefreshToken: newTokenMaterial.RawToken,
            RefreshTokenExpiresAtUtc: newTokenExpiresAtUtc);
    }

    private async Task RevokeFamilyAsync(Guid familyId, DateTime revokedAtUtc, CancellationToken cancellationToken)
    {
        await _transactionManager.ExecuteAsync(async ct =>
        {
            await _refreshTokenRepository.RevokeActiveFamilyAsync(familyId, revokedAtUtc,
                RefreshTokenRevocationReason.ReuseDetected, ct);

            await _dbContext.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}