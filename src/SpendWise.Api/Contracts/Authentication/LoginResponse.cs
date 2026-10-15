namespace SpendWise.Api.Contracts.Authentication;

public sealed record LoginResponse(string AccessToken, string TokenType, int ExpiresIn);