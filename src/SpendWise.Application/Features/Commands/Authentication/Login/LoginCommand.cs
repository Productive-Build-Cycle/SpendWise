using MediatR;

namespace SpendWise.Application.Features.Commands.Authentication.Login;

public sealed record LoginCommand(string UserName, string Password) : IRequest<LoginCommandResult>;