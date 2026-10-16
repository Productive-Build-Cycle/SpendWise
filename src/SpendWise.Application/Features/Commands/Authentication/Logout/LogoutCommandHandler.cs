using MediatR;
using SpendWise.Application.Common.Enums;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Commands.Authentication.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITransactionManager _transactionManager;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService, IApplicationDbContext dbContext,
        ITransactionManager transactionManager)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _dbContext = dbContext;
        _transactionManager = transactionManager;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RawRefreshToken))
            return Unit.Value;

        string tokenHash;

        try
        {
            tokenHash = _refreshTokenService.Hash(request.RawRefreshToken);
        }
        catch (ArgumentException)
        {
            return Unit.Value;
        }

        var token = await _refreshTokenRepository.FindByTokenHashAsync(tokenHash, cancellationToken);


        if (token is null)
            return Unit.Value;


        for (var attempt = 0;; attempt++)
        {
            try
            {
                var revokedAtUtc = DateTime.UtcNow;

                await _transactionManager.ExecuteAsync(async ct =>
                {
                    await _refreshTokenRepository.RevokeActiveFamilyAsync(token.FamilyId, revokedAtUtc,
                        RefreshTokenRevocationReason.Logout, ct);

                    await _dbContext.SaveChangesAsync(ct);
                }, cancellationToken);

                return Unit.Value;
            }
            catch (ConcurrencyConflictException) when (attempt == 0)
            {
                // Retry using the latest database state.
            }
        }
    }
}