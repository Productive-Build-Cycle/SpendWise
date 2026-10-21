namespace SpendWise.Application.Features.Commands.Authentication.Refresh;

public sealed record RefreshTokenCommandResult(
    bool Succeeded,
    string? AccessToken = null,
    int ExpiresInSeconds = 0,
    string? RawRefreshToken = null,
    DateTime? RefreshTokenExpiresAtUtc = null);