using MediatR;
using SpendWise.Application.Common.Exceptions;
using SpendWise.Application.Common.Models;
using SpendWise.Application.Interfaces;

namespace SpendWise.Application.Features.Commands.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginCommandResult>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITransactionManager _transactionManager;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext dbContext,
        ITransactionManager transactionManager)
    {
        _identityService = identityService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _dbContext = dbContext;
        _transactionManager = transactionManager;
    }

    public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userId = await _identityService.AuthenticateAsync(request.UserName, request.Password, cancellationToken);

        if (userId is null || userId == Guid.Empty)
            throw new InvalidCredentialsException();

        var now = DateTime.UtcNow;
        var refreshTokenExpiresAtUtc = now.Add(_refreshTokenService.Lifetime);

        var refreshTokenMaterial = _refreshTokenService.Generate();

        var refreshTokenRecord = new RefreshTokenRecord(
            Id: Guid.NewGuid(),
            UserId: userId.Value,
            TokenHash: refreshTokenMaterial.TokenHash,
            FamilyId: Guid.NewGuid(),
            CreatedAtUtc: now,
            ExpiresAtUtc: refreshTokenExpiresAtUtc);

        var accessToken = _jwtService.CreateAccessToken(userId.Value);

        // Persist the refresh token before returning the raw token to the API.
        // If persistence fails, no cookie is issued to the client.
        await _transactionManager.ExecuteAsync(
            async ct =>
            {
                _refreshTokenRepository.Add(refreshTokenRecord);

                await _dbContext.SaveChangesAsync(ct);
            },
            cancellationToken);

        return new LoginCommandResult(
            AccessToken: accessToken.Token,
            ExpiresInSeconds: accessToken.ExpiresInSeconds,
            RawRefreshToken: refreshTokenMaterial.RawToken,
            RefreshTokenExpiresAtUtc: refreshTokenExpiresAtUtc);
    }
}