namespace SpendWise.Application.Features.Commands.Authentication.Login;

public sealed record LoginCommandResult(
    string AccessToken,
    int ExpiresInSeconds,
    string RawRefreshToken,
    DateTime RefreshTokenExpiresAtUtc);