using MediatR;

namespace SpendWise.Application.Features.Commands.Authentication.Refresh;

public sealed record RefreshTokenCommand(string RawRefreshToken) : IRequest<RefreshTokenCommandResult>;