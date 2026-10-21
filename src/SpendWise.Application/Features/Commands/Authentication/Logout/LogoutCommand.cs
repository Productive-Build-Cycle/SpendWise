using MediatR;

namespace SpendWise.Application.Features.Commands.Authentication.Logout;

public sealed record LogoutCommand(string? RawRefreshToken) : IRequest<Unit>;